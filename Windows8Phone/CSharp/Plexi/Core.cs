using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Plexi.Events;
using Plexi.Models;
using Plexi.Util;

namespace Plexi
{
    public interface IPlexiService
    {
        string OriginalQuery { get; }

        void ClearContext();

        void ResetTimer();

        void Query(string query);

        void Choice(ChoiceModel choice);

        Task<Dictionary<string, object>> RegisterUser(string accountName, string password);

        Task<Dictionary<string, object>> LoginUser(string accountName, string password);

        void LogoutUser();

        string GetAuthToken();

        void StoreAuthToken(string token);

        Task<Dictionary<string, object>> GetDeviceInfo();

        event EventHandler<ShowEventArgs> Show;

        event EventHandler<ActorEventArgs> Act;

        event EventHandler<ChoiceEventArgs> Choose;

        event EventHandler<ErrorEventArgs> Error;

        event EventHandler<RegisterEventArgs> Register;

        event EventHandler<ProgressEventArgs> InProgress;

        event EventHandler<NotificationEventArgs> PushNotificationReceived;
    }

    public class Core : IPlexiService
    {
        private static string CLASSIFIER = Resources.PlexiResources.Classifier;

        private static string DISAMBIGUATOR = Resources.PlexiResources.Disambiguator;

        private static string RESPONDER = Resources.PlexiResources.Auditor;

        private static string PUD = Resources.PlexiResources.Pud;

        private static string REGISTRATION = Resources.PlexiResources.Registration;

        private static string LOGIN = Resources.PlexiResources.Login;

        // used by auditor
        private string[] auditorStates = new string[] { "disambiguate", "inprogress", "choice" };

        // used by BuildDateTime
        private List<Tuple<string, string>> datetimes = new List<Tuple<string, string>>();

        // used in RequestHelper to benchmark request times
        private Stopwatch stopWatch;

        // gets completed with all the necessary fields in order to fulfill an action
        private ClassifierModel mainContext = null;

        // indicates fields that need to be completed in the main context
        private ResponderModel tempContext = null;

        private StateModel currentState = new StateModel();

        private DispatcherTimer contextTimer;

        public string OriginalQuery { get; private set; }

        public event EventHandler<ShowEventArgs> Show;

        public event EventHandler<ActorEventArgs> Act;

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<RegisterEventArgs> Register;

        public event EventHandler<ChoiceEventArgs> Choose;

        public event EventHandler<ProgressEventArgs> InProgress;

        public event EventHandler<NotificationEventArgs> PushNotificationReceived;

        public Core()
        {
            Debug.WriteLine("plexi core initialized");

            /*
            IPushService pushService = new PushService();

            // listen for push notifications
            pushService.NotificationReceived += PushNotificationReceived;
            */

            // listen for Plexi state changes 
            this.currentState.PropertyChanged += OnStateChanged;

            CreateTimer();

            if (this.stopWatch == null)
            {
                this.stopWatch = new Stopwatch();
            }
        }

        public async Task<Dictionary<string, object>> RegisterUser(string accountName, string password)
        {
            Dictionary<string, object> postData = new Dictionary<string, object>();

            byte[] duidAsBytes = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];

            string duid = Convert.ToBase64String(duidAsBytes);

            //postData.Add("device_id", duid);
            //postData.Add("user_id", UserExtendedProperties.GetValue("ANID2"));
            postData.Add("username", accountName);
            postData.Add("password", password);

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(REGISTRATION, "POST", postData);

            Debug.WriteLine(String.Format("RegisterUser Response: {0}", SerializeData(response)));

            return response;
        }

        public async Task<Dictionary<string, object>> LoginUser(string accountName, string password)
        {
            byte[] duidAsBytes = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];

            string duid = Convert.ToBase64String(duidAsBytes);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add(Resources.PlexiResources.AuthDeviceHeader, duid);

            Dictionary<string, object> postData = new Dictionary<string, object>();

            postData.Add("username", accountName);
            postData.Add("password", password);
            //postData.Add("device_id", duid);

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(LOGIN, "POST", postData, headers);

            Debug.WriteLine(String.Format("LoginUser Response: {0}", SerializeData(response)));

            if (response.ContainsKey("token"))
            {
                string token = (string)response["token"];

                StoreAuthToken(token);
            }
    
            return response;
        }

        public void LogoutUser()
        {
            //throw new NotImplementedException();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            settings.Remove(Resources.PlexiResources.SettingsAuthKey);
        }

        public void Query(string query)
        {
            this.OriginalQuery = query;

            string newState;

            switch (currentState.State)
            {
                case "inprogress":
                case "error":
                    newState = "disambiguate:active"; 
                    break;

                case "choice":
                    newState = "disambiguate:candidate";
                    break;

                default:
                    newState = "init";
                    break;
            }

            this.currentState.Set(newState, query);
        }

        public void ResetTimer()
        {
            this.contextTimer.Stop();
            CreateTimer();
        }

        public void ClearContext()
        {
            this.mainContext = null;
            this.tempContext = null;
            this.currentState.Reset();

            ResetTimer();
        }

        private async void OnStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                switch (currentState.State)
                {
                    case "init":
                        await Classify((string)currentState.Response);
                        break;

                    case "audit":
                        await Auditor((ClassifierModel)currentState.Response);
                        break;

                    case "disambiguate":
                        DisambiguatePassive((ResponderModel)currentState.Response);
                        break;

                    case "disambiguate:personal":
                        //ActorInterceptor(mainContext.model);

                        DisambiguatePersonal((ResponderModel)currentState.Response);
                        break;

                    case "disambiguate:active":
                        DisambiguateActive((string)currentState.Response);
                        break;

                    case "disambiguate:candidate":
                        DisambiguateCandidate((string)currentState.Response);
                        break;

                    case "inprogress":
                    case "error":
                        ShowDialog((ResponderModel)currentState.Response);
                        break;

                    case "choice":
                        ChoiceList((ResponderModel)currentState.Response);
                        break;

                    case "restart":
                        Restart((ResponderModel)currentState.Response);
                        break;

                    case "completed":
                        Actor((ResponderModel)currentState.Response);
                        break;

                    case "exception":
                        ErrorMessage((string)currentState.Response);
                        break;
                    
                    case "noaccount":

                        break;
                }
            }
        }

        private void CreateTimer()
        {
            this.contextTimer = new DispatcherTimer();
            this.contextTimer.Interval = TimeSpan.FromMinutes(2);
            this.contextTimer.Tick += Reset;
        }

        private void Reset(object sender, EventArgs e)
        {
            ClearContext();
        }

        private void ChoiceList(ResponderModel response)
        {
            try
            {
                Dictionary<string, object> simple = response.show.simple;

                if (simple.ContainsKey("list"))
                {
                    EventHandler<ChoiceEventArgs> handler = Choose;

                    if (handler != null)
                    {
                        handler(this, new ChoiceEventArgs(response));
                    }
                }
                else
                {
                    Debug.WriteLine("no choice list could be found");
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        // NOTE: called from inprogress status
        private void ShowDialog(ResponderModel response)
        {
            try
            {
                ShowDialog(response.show, response.speak);
            }
            catch (Exception err)
            {
                Debug.WriteLine("Show:inprogress - " + err.Message);
            }
        }

        // NOTE: called from Actor method
        private void ShowDialog(ShowModel showModel, string speak = "")
        {
            if (showModel.simple.ContainsKey("text"))
            {
                string show = (string)showModel.simple["text"];

                string link = null;

                if (showModel.simple.ContainsKey("link"))
                    link = (string)showModel.simple["link"];

                ShowDialog("please", speak, show, link);
            }
        }

        private void ShowDialog(string type, string speak = "", string show = null, string link = null)
        {
            EventHandler<ShowEventArgs> handler = Show;

            if (handler != null)
            {
                handler(this, new ShowEventArgs(speak, show, link, this.currentState.State));
            }
        }

        private void RegisterMessage(string classificationModel)
        {
            EventHandler<RegisterEventArgs> handler = Register;

            if (handler != null)
            {
                handler(this, new RegisterEventArgs(classificationModel));
            }
        }

        private void ErrorMessage(string message, bool isConfirm = false)
        {
            EventHandler<ErrorEventArgs> handler = Error;

            if (handler != null)
            {
                handler(this, new ErrorEventArgs(message));
            }
        }

        private async Task Classify(string query)
        {
            try
            {
                string endpoint = String.Format("{0}?query={1}", CLASSIFIER, query);

                ClassifierModel classifierResults = await RequestHelper<ClassifierModel>(endpoint, "GET");

                if (classifierResults.error != null)
                {
                    currentState.Set("exception", classifierResults.error.message);
                }
                else
                {
                    ClassifierModel context = await DoClientOperations(classifierResults, classifierResults.payload);

                    currentState.Set("audit", context);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private async void DisambiguateActive(string data)
        {
            string field = tempContext.field;

            string type = tempContext.type;

            string payload = data;

            DisambiguatorModel postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;

            try
            {
                string endpoint = String.Format("{0}/active", DISAMBIGUATOR);

                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, null, true);

                // hand off response to disambig response handler
                DisambiguateResponseHandler(response, field, type);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private async void DisambiguateCandidate(string data)
        {
            Dictionary<string, object> simple = tempContext.show.simple;

            string field = tempContext.field;

            string type = tempContext.type;

            JArray list = (simple.ContainsKey("list")) ? (JArray)simple["list"] : new JArray();

            string payload = data;

            DisambiguatorModel postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.candidates = list;

            try
            {
                string endpoint = String.Format("{0}/candidate", DISAMBIGUATOR);

                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, null, true);

                // hand off response to disambig response handler
                DisambiguateResponseHandler(response, field, type);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private async void DisambiguatePassive(ResponderModel data)
        {
            string field = data.field;

            string type = data.type;

            object payload;

            if (field.Contains("."))
            {
                payload = Find(field);
            }
            else
            {
                payload = this.mainContext.payload[field];
            }

            Dictionary<string, object> deviceInfo = new Dictionary<string, object>();

            try
            {
                deviceInfo = await GetDeviceInfo();
            }
            catch (Exception err)
            {
                ErrorMessage(err.Message);
                return;
            }

            DisambiguatorModel postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.device_info = deviceInfo;

            Debug.WriteLine("disambig passive");
            Debug.WriteLine(SerializeData(postData, true));

            String endpoint = String.Format("{0}/passive", DISAMBIGUATOR);

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, null, true);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        private async void DisambiguatePersonal(ResponderModel data)
        {
            string field = data.field;

            string type = data.type;

            object payload;

            if (field.Contains("."))
            {
                payload = Find(field);
            }
            else
            {
                payload = this.mainContext.payload[field];
            }

            DisambiguatorModel postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;

            String endpoint = String.Format("{0}/disambiguate", PUD);

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        private async void DisambiguateResponseHandler(Dictionary<string, object> response, string field, string type)
        {
            if (response != null)
            {
                if (response.ContainsKey("error"))
                {
                    ErrorModel er = (ErrorModel)response["error"];

                    currentState.Set("exception", er.message);
                }
                else
                {
                    ClassifierModel context = this.mainContext.DeepCopy<ClassifierModel>();

                    context = await DoClientOperations(context, response);

                    if (response.ContainsKey(type))
                    {
                        if (field.Contains("."))
                        {
                            context = Replace(context, field, response[type]);
                        }
                        else
                        {
                            context.payload[field] = response[type];
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Disambiguation response is missing type");
                    }

                    this.currentState.Set("audit", context);
                }
            }
            else
            {
                Debug.WriteLine("oops no responder response");
            }
        }

        private async Task Auditor(ClassifierModel context)
        {
            if (!context.Equals(this.mainContext))
            {
                this.mainContext = context;

                string endpoint = String.Format("{0}/audit", RESPONDER);

                ResponderModel response = await RequestHelper<ResponderModel>(endpoint, "POST", context);

                if (response.error != null)
                {
                    this.currentState.Set("exception", response.error.msg);
                }
                else
                {
                    string state = response.status.Replace(" ", "");

                    string crossCheck = state.Split(':')[0];

                    if (this.auditorStates.Contains(crossCheck))
                    {
                        tempContext = response;
                        contextTimer.Start();
                    }

                    // create event and trigger based on status
                    this.currentState.Set(state, response);
                }
            }
            else
            {
                Debug.WriteLine("potential request loop detected");
            }
        }

        public void Choice(ChoiceModel choice)
        {   
            string field = tempContext.field;

            Dictionary<string, object> t = choice.data;

            ClassifierModel context = this.mainContext.DeepCopy<ClassifierModel>();

            if (field.Contains("."))
            {
                context = Replace(this.mainContext, field, choice.data);
            }
            else
            {
                context.payload[field] = choice.data;
            }

            this.currentState.Set("audit", context);
        }

        private void Restart(ResponderModel data)
        {
            if (data.data == null)
            {
                Debug.WriteLine("missing new replacement context");
                return;
            }

            this.currentState.Set("audit", data.data);
        }

        private async void Actor(ResponderModel data)
        {
            string actor = data.actor;

            if (actor != null)
            {
                string endpoint = String.Format("{0}/actors/{1}", RESPONDER, actor);

                Dictionary<string, string> headers = null;

                if (actor.Contains("private:"))
                {
                    endpoint = String.Format("{0}/actors/{1}", PUD, actor.Replace("private:", ""));

                    string authToken;

                    try
                    {
                        authToken = GetAuthToken();
                    }
                    catch (KeyNotFoundException keyErr)
                    {
                        // prompt user to signup for a stremor account
                        RegisterMessage(data.data.model);
                        return;
                    }

                    headers = new Dictionary<string, string>();
                    headers.Add(Resources.PlexiResources.AuthTokenHeader, authToken);
                }

                ActorModel response = await RequestHelper<ActorModel>(endpoint, "POST", mainContext, headers);

                ClearContext();

                if (response.error != null)
                {
                    this.currentState.Set("exception", response.error.msg);
                }
                else
                {
                    EventHandler<ActorEventArgs> handler = Act;

                    if (handler != null)
                    {
                        handler(this, new ActorEventArgs(response));
                    }
                }
            }
            else
            {
                // we have no actor, so send responder object to show
                ShowDialog(data);
            }

            return;
        }

        /*
        // these are actor's that will be handled locally. no need to run out to web service
        // all the data needed to fulfill the tasks should be in the mainContext var.
        private void ActorInterceptor(string actor)
        {
            Dictionary<string, object> payload = this.mainContext.payload;

            if (!SimpleIoc.Default.IsRegistered<ITaskService>())
            {
                SimpleIoc.Default.Register<ITaskService, TaskService>();
            }

            ITaskService tasks = SimpleIoc.Default.GetInstance<ITaskService>();

            tasks.MainContext = this.mainContext;

            switch (actor)
            {
                case "time":
                    tasks.ShowClock();
                    break;

                case "email":
                case "email_denial":
                    tasks.ComposeEmail(payload);
                    break;

                case "sms":
                case "sms_denial":
                    tasks.ComposeSms(payload);
                    break;

                case "directions":
                case "directions_denial":
                    tasks.GetDirections();
                    break;

                case "call":
                case "call_denial":
                    tasks.PhoneCall(payload);
                    break;

                case "calendar":
                case "calendar_create":
                case "calendar_create_denial":
                    tasks.SetAppointment();
                    break;

                case "information":

                    break;

                case "alarm":
                    tasks.SetAlarm();
                    break;

                case "reminder":
                    tasks.SetReminder();
                    break;
            }

            return;
        }
        */

        #region helpers
        private async Task<T> RequestHelper<T>(string endpoint, string method, object data = null, Dictionary<string, string> headers = null, bool includeNulls = false)
        {
            Request req = new Request();

            req.Method = method;

            if (headers != null)
            {
                req.Headers = headers;
            }

            EventHandler<ProgressEventArgs> handler = InProgress;

            // Notify the listener that the request has begun
            if (handler != null)
            {
                handler(this, new ProgressEventArgs(true));
            }
           
            T response;

            this.stopWatch.Start();

            if (method.ToLower() == "get")
            {
                response = await req.DoRequestJsonAsync<T>(endpoint);
            }
            else
            {
                req.ContentType = "application/json";

                response = await req.DoRequestJsonAsync<T>(endpoint, SerializeData(data, includeNulls));
            }

            this.stopWatch.Stop();

            Debug.WriteLine(String.Format("this request took {0} to complete", stopWatch.Elapsed));

            this.stopWatch.Reset();

            // Notify the listener that the request is complete
            if (handler != null)
            {
                handler(this, new ProgressEventArgs(false));
            }

            return response;
        }

        public string GetAuthToken()
        {
            string key = Resources.PlexiResources.SettingsAuthKey;
            
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(Resources.PlexiResources.SettingsAuthKey))
            {
                throw new KeyNotFoundException("no auth token could be found");
            }

            byte[] tokenBytes = (byte[])settings[key];

            return Security.Decrypt(tokenBytes);
        }

        // TODO: handle increase quota issue
        public void StoreAuthToken(string token)
        {
            byte[] byteToken = Security.Encrypt(token);

            string key = Resources.PlexiResources.SettingsAuthKey;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            try
            {
                settings[key] = byteToken;
            }
            catch (KeyNotFoundException)
            {
                settings.Add(key, byteToken);  
            }
            catch (ArgumentException)
            {
                settings.Add(key, byteToken);  
            }

            settings.Save();
        }

        private async Task<ClassifierModel> DoClientOperations(ClassifierModel context, Dictionary<string, object> response)
        {
            response = await ReplaceLocation(response);
            response = BuildDateTime(response);
            context = PrependTo(context, response);

            return context;
        }

        private ClassifierModel PrependTo(ClassifierModel context, Dictionary<string, object> data)
        {
            try
            {
                if (!data.ContainsKey("unused_tokens"))
                {
                    return context;
                }

                if (((JArray)data["unused_tokens"]).Count <= 0)
                {
                    return context;
                }

                string prepend = (string)((JArray)data["unused_tokens"]).Aggregate((i, j) => i + " " + j);

                string field = (string)data["prepend_to"];

                string payloadField = "";

                if (context.payload.ContainsKey(field) && context.payload[field] != null)
                {
                    payloadField = " " + (string)context.payload[field];
                }

                context.payload[field] = prepend + payloadField;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            return context;
        }

        protected ClassifierModel Replace(ClassifierModel context, string field, object type)
        {
            List<string> fields = field.Split('.').ToList();

            string last = fields[fields.Count - 1];

            // convert to generic object
            object obj = context.DeepCopy<object>();

            object t = fields.Aggregate(obj, (a, b) =>
            {
                if (b == last)
                {
                    return ((JObject)a)[b] = JToken.FromObject(type);
                }
                else
                {
                    return ((JObject)a)[b];
                }
            }
            );

            // convert back to classifier model
            return obj.DeepCopy<ClassifierModel>();
        }

        protected object Find(string field)
        {
            List<string> fields = field.Split('.').ToList();

            // convert to generic object
            object context = this.mainContext.DeepCopy<object>();

            return fields.Aggregate(context, (a, b) => ((JObject)a)[b]);
        }

        private async Task<Dictionary<string, object>> ReplaceLocation(Dictionary<string, object> payload)
        {
            try
            {
                if (payload.ContainsKey("location") && payload["location"] != null)
                {
                    if (payload["location"].GetType() == typeof(string))
                    {
                        string location = (string)payload["location"];
                        if (location.Contains("current_location"))
                        {
                            //Debug.WriteLine(SerializeData(GetDeviceInfo()));
                            payload["location"] = await GetDeviceInfo();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            return payload;
        }

        private Dictionary<string, object> BuildDateTime(Dictionary<string, object> data)
        {
            try
            {
                if (data != null)
                {
                    if (this.datetimes.Count <= 0)
                    {
                        this.datetimes.Add(new Tuple<string, string>("date", "time"));
                        this.datetimes.Add(new Tuple<string, string>("start_date", "start_time"));
                        this.datetimes.Add(new Tuple<string, string>("end_date", "end_time"));
                    }

                    foreach (Tuple<string, string> datetime in datetimes)
                    {
                        if (data.ContainsKey(datetime.Item1) || data.ContainsKey(datetime.Item2))
                        {
                            bool removeDate = false;
                            bool removeTime = false;

                            // add placeholders to satisfy builder
                            if (!data.ContainsKey(datetime.Item1))
                            {
                                data[datetime.Item1] = null;
                                removeDate = true;
                            }

                            if (!data.ContainsKey(datetime.Item2))
                            {
                                data[datetime.Item2] = null;
                                removeTime = true;
                            }

                            // perform replacement
                            if (data[datetime.Item1] != null || data[datetime.Item2] != null)
                            {
                                Dictionary<string, string> build = Datetime.BuildDatetimeFromJson(data[datetime.Item1], data[datetime.Item2]);

                                Debug.WriteLine("datetime build - " + build["date"] + " " + build["time"]);

                                if (data[datetime.Item1] != null)
                                    data[datetime.Item1] = build["date"];

                                if (data[datetime.Item2] != null)
                                    data[datetime.Item2] = build["time"];
                            }

                            // cleanup
                            if (removeDate == true)
                            {
                                data.Remove(datetime.Item1);
                            }

                            if (removeTime == true)
                            {
                                data.Remove(datetime.Item2);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("BuildDateTime Error - " + err.Message);
            }

            Debug.WriteLine("after build datetime");
            Debug.WriteLine(SerializeData(data));

            return data;
        }

        public async Task<Dictionary<string, object>> GetDeviceInfo()
        {
            Dictionary<string, object> deviceInfo = new Dictionary<string, object>();
            
            deviceInfo["timestamp"] = Datetime.ConvertToUnixTimestamp(DateTime.Now);
            deviceInfo["timeoffset"] = DateTimeOffset.Now.Offset.Hours;

            Dictionary<string, object> geolocation = LocationService.CurrentPosition;
            // geo fell down so 'manually' get geolocation
            if (geolocation == null)
            {
                geolocation = await LocationService.GetGeolocation();
            }

            if (!geolocation.ContainsKey("error") && geolocation.Count > 1)
            {
                deviceInfo["latitude"] = geolocation["latitude"];
                deviceInfo["longitude"] = geolocation["longitude"];
            }
            else if (geolocation.ContainsKey("error"))
            {
                // ran into error acquiring geolocation
                // prob wanna throw an exception in the Location object
                Debug.WriteLine(geolocation["error"]);
            }

            return deviceInfo;
        }

        private string SerializeData(object data, bool includeNulls = false)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Include;
            jsonSettings.NullValueHandling = (includeNulls == true) ? NullValueHandling.Include : NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(data, jsonSettings);
        }
        #endregion
    }
}
