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
    public enum State 
    { 
        Uninitialized, 
        Init,
        Audit, 
        Disambiguate, 
        DisambiguatePersonal, 
        DisambiguateActive, 
        DisambiguateCandidate,
        InProgress,
        Error,
        Choice,
        Restart,
        Completed,
        NoAccount,
        Exception
    };

    public interface IPlexiService
    {
        /// <summary>
        /// Get the original query that initiated the request
        /// </summary>
        string OriginalQuery { get; }

        /// <summary>
        /// Get the current state of Plexi
        /// </summary>
        State State { get; }

        /// <summary>
        /// Clears the current request and any data currently being retained
        /// </summary>
        void ClearContext();

        /// <summary>
        /// Resets the idle dialog timer. If no user response is received within 2 minutes, a call to ClearContext will occur
        /// </summary>
        void ResetTimer();

        /// <summary>
        /// Either starts a new request or handles additional information depending on the current state 
        /// </summary>
        /// <param name="query"></param>
        void Query(string query);

        /// <summary>
        /// Handles the item selected by the user from a list of items
        /// </summary>
        /// <param name="choice"></param>
        void Choice(ChoiceModel choice);

        /// <summary>
        /// Provides a mechanism for registering a new user.
        /// <para>Note: this should only be used if you plan to have Plexi manage your users</para>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<RegisterModel> RegisterUser(string email, string accountName, string password);

        /// <summary>
        /// Provides a mechanism for logging a user in
        /// <para>Note: this should only be used if you plan to have Plexi manage your users</para>
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<LoginModel> LoginUser(string accountName, string password);

        /// <summary>
        /// Provides a mechanism for logging a user out
        /// <para>Note: this should only be used if you plan to have Plexi manage your users</para>
        /// </summary>
        void LogoutUser();

        Task<Dictionary<string, object>> ForgotPassword(string id);

        /// <summary>
        /// Provides a mechanism for retrieving an decrypted auth token when stored with a call to StoreAuthToken
        /// </summary>
        /// <returns></returns>
        string GetAuthToken();

        /// <summary>
        /// Provides a mechanism to encrypt and store an auth token received from LoginUser
        /// </summary>
        /// <param name="token"></param>
        void StoreAuthToken(string token);

        /// <summary>
        /// Provides a mechanism to retrieve device location information
        /// </summary>
        /// <returns></returns>
        Task<LocationModel> GetDeviceInfo();

        Task<List<AccountModel>> GetAccounts();

        void RemoveAccount(long id);

        /// <summary>
        /// Occurs when dialog is to be shown to the user
        /// </summary>
        event EventHandler<ShowEventArgs> Show;

        /// <summary>
        /// Occurs when Plexi is ready to  
        /// </summary>
        event EventHandler<ActorEventArgs> Act;

        /// <summary>
        /// Occurs when a decision is needed from the user
        /// </summary>
        event EventHandler<ChoiceEventArgs> Choose;

        /// <summary>
        /// Occurs when an error was returned from Plexi
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Occurs when authorization to personal data is required
        /// </summary>
        event EventHandler<AuthorizationEventArgs> Authorize;

        /// <summary>
        /// Occurs when more information is need from the user to fulfill the request
        /// </summary>
        event EventHandler<ProgressEventArgs> InProgress;

        /// <summary>
        /// Occurs when the display data is returned from Plexi's default Actor
        /// </summary>
        event EventHandler<CompleteEventArgs> Complete;

        /// <summary>
        /// Occurs when a push notification has been received from Plexi
        /// </summary>
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

        private static string FORGOTPASSWORD = Resources.PlexiResources.Password;

        private static string ACCOUNTS = Resources.PlexiResources.Accounts;

        private static string ACCOUNT = Resources.PlexiResources.Account;

        private static Dictionary<string, State> StateMap = new Dictionary<string, State>()
        {
            {"init", State.Init },
            {"audit", State.Audit },
            {"disambiguate", State.Disambiguate },
            {"disambiguate:personal", State.DisambiguatePersonal },
            {"disambiguate:active", State.DisambiguateActive },
            {"disambiguate:candidate", State.DisambiguateCandidate },
            {"inprogress", State.InProgress },
            {"error", State.Error },
            {"choice", State.Choice },
            {"restart", State.Restart },
            {"completed", State.Completed },
            {"noaccount", State.NoAccount },
            {"exception", State.Exception },
        };

        // used by auditor
        private string[] auditorStates = new string[] { "disambiguate", "inprogress", "choice" };

        // used by BuildDateTime
        private List<Tuple<string, string>> datetimes = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("date", "time"),
                new Tuple<string, string>("start_date", "start_time"),
                new Tuple<string, string>("end_date", "end_time")
            };

        // used in RequestHelper to benchmark request times
        private Stopwatch stopWatch;

        // gets completed with all the necessary fields in order to fulfill an action
        private ClassifierModel mainContext = null;

        // indicates fields that need to be completed in the main context
        private ResponderModel tempContext = null;

        private StateModel currentState = new StateModel(State.Uninitialized, null);

        private DispatcherTimer contextTimer;

        public string OriginalQuery { get; private set; }

        public State State { get { return currentState.State; } }

        public event EventHandler<ShowEventArgs> Show;

        public event EventHandler<ActorEventArgs> Act;

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<AuthorizationEventArgs> Authorize;

        public event EventHandler<ChoiceEventArgs> Choose;

        public event EventHandler<ProgressEventArgs> InProgress;

        public event EventHandler<CompleteEventArgs> Complete;

        public event EventHandler<NotificationEventArgs> PushNotificationReceived;

        public Core()
        {
            /*
            IPushService pushService = new PushService();

            // listen for push notifications
            pushService.NotificationReceived += PushNotificationReceived;
            */

            // kick off geolocation listener 
            LocationService.Default.StartTrackingGeolocation();

            CreateTimer();

            if (this.stopWatch == null)
            {
                this.stopWatch = new Stopwatch();
            }
        }

        public async Task<RegisterModel> RegisterUser(string email, string accountName, string password)
        {
            Dictionary<string, object> postData = new Dictionary<string, object>();

            //postData.Add("device_id", getDuid());
            //postData.Add("user_id", UserExtendedProperties.GetValue("ANID2"));
            postData.Add("email", email);
            postData.Add("username", accountName);
            postData.Add("password", password);

            RegisterModel response = await RequestHelper<RegisterModel>(REGISTRATION, "POST", postData);

            Debug.WriteLine(String.Format("RegisterUser Response: {0}", SerializeData(response)));

            return response;
        }

        public async Task<LoginModel> LoginUser(string accountName, string password)
        {
            string duid = GetDuid();

            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add(Resources.PlexiResources.AuthDeviceHeader, duid);

            Dictionary<string, object> postData = new Dictionary<string, object>();

            postData.Add("username", accountName);
            postData.Add("password", password);
            //postData.Add("device_id", duid);

            LoginModel response = await RequestHelper<LoginModel>(LOGIN, "POST", postData, headers);

            Debug.WriteLine(String.Format("LoginUser Response: {0}", SerializeData(response)));

            if (response.error != null)
            {
                // houston we have a problem
            }
            else
            {
                StoreAuthToken(response.token);
            }
    
            return response;
        }

        public void LogoutUser()
        {
            //throw new NotImplementedException();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            settings.Remove(Resources.PlexiResources.SettingsAuthKey);

            settings.Save();
        }

        public async Task<Dictionary<string, object>> ForgotPassword(string id)
        {
            Dictionary<string, object> postData = new Dictionary<string, object>();

            postData.Add("id", id);

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(FORGOTPASSWORD, "POST", postData);

            Debug.WriteLine(String.Format("Forgot Password Response: {0}", SerializeData(response)));

            return response;
        }

        public void Query(string query)
        {
            this.OriginalQuery = query;

            Plexi.State currState = currentState.State;
            Plexi.State newState;

            switch (currState)
            {
                case Plexi.State.InProgress:
                case Plexi.State.Error:
                    newState = Plexi.State.DisambiguateActive; 
                    break;

                case Plexi.State.Choice:
                    newState = Plexi.State.DisambiguateCandidate;
                    break;

                default:
                    newState = Plexi.State.Init;
                    break;
            }

            ChangeState(newState, query);
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

        private async void ChangeState(State state, object data)
        {
            currentState.Set(state, data);

            switch (state)
            {
                case State.Init:
                    await Classify((string)currentState.Response);
                    break;

                case State.Audit:
                    await Auditor((ClassifierModel)currentState.Response);
                    break;

                case State.Disambiguate:
                    DisambiguatePassive((ResponderModel)currentState.Response);
                    break;

                case State.DisambiguatePersonal:
                    DisambiguatePersonal((ResponderModel)currentState.Response);
                    break;

                case State.DisambiguateActive:
                    DisambiguateActive((string)currentState.Response);
                    break;

                case State.DisambiguateCandidate:
                    DisambiguateCandidate((string)currentState.Response);
                    break;

                case State.InProgress:
                case State.Error:
                    ShowDialog((ResponderModel)currentState.Response);
                    break;

                case State.Choice:
                    ChoiceList((ResponderModel)currentState.Response);
                    break;

                case State.Restart:
                    Restart((ResponderModel)currentState.Response);
                    break;

                case State.Completed:
                    Actor((ResponderModel)currentState.Response);
                    break;

                case State.Exception:
                    ErrorMessage((string)currentState.Response);
                    break;

                case State.NoAccount:
                    NoAccount((ResponderModel)currentState.Response);
                    break;
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

        private void ShowDialog(ResponderModel response)
        {
            try
            {
                ShowDialog(response.show, response.speak);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("ShowDialog Error:{0}", err.Message));
            }
        }

        private void ShowDialog(ShowModel showModel, string speak = "")
        {
            if (showModel.simple.ContainsKey("text"))
            {
                string show = (string)showModel.simple["text"];

                string link = null;

                if (showModel.simple.ContainsKey("link"))
                    link = (string)showModel.simple["link"];

                ShowDialog(speak, show, link);
            }
        }

        private void ShowDialog(string speak = "", string show = null, string link = null)
        {
            EventHandler<ShowEventArgs> handler = Show;

            if (handler != null)
            {
                handler(this, new ShowEventArgs(speak, show, link, this.currentState.State));
            }
        }

        private void AuthorizationMessage(string classificationModel)
        {
            EventHandler<AuthorizationEventArgs> handler = Authorize;

            if (handler != null)
            {
                handler(this, new AuthorizationEventArgs(classificationModel));
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
                    ChangeState(State.Exception, classifierResults.error.message);
                }
                else
                {
                    ClassifierModel context = await DoClientOperations(classifierResults, classifierResults.payload);

                    ChangeState(State.Audit, context);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("Classifier Error:{0}", err.Message));
                Debug.WriteLine(err.InnerException.Message);
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

                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, true);

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

                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, true);

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

            LocationModel deviceInfo = new LocationModel();

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
            postData.device_info = new Dictionary<string, object>() 
            { 
                { "timestamp", deviceInfo.timestamp },
                { "timeoffset", deviceInfo.timeoffset.Hours },
                { "latitude", deviceInfo.geoCoordinate.Latitude },
                { "longitude", deviceInfo.geoCoordinate.Longitude }
            };

            String endpoint = String.Format("{0}/passive", DISAMBIGUATOR);

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, true);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        private async void DisambiguatePersonal(ResponderModel data)
        {
            string authToken;

            try
            {
                authToken = GetAuthToken();
            }
            catch (KeyNotFoundException keyErr)
            {
                Debug.WriteLine(String.Format("DisambiguatePersonal:{0}", keyErr.Message));
                return;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(Resources.PlexiResources.AuthTokenHeader, authToken);
            headers.Add(Resources.PlexiResources.AuthDeviceHeader, GetDuid());

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

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(endpoint, "POST", postData, headers);

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

                    ChangeState(State.Exception, er.message);
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

                    ChangeState(State.Audit, context);
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
                    ChangeState(State.Exception, response.error.msg);
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
                    ChangeState(StateMap[state], response);
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

            ChangeState(State.Audit, context);
        }

        private void Restart(ResponderModel data)
        {
            if (data.data == null)
            {
                Debug.WriteLine("missing new replacement context");
                return;
            }

            ChangeState(State.Audit, data.data);
        }

        private void NoAccount(ResponderModel response)
        {
            EventHandler<AuthorizationEventArgs> handler = Authorize;

            if (handler != null)
            {
                handler(this, new AuthorizationEventArgs());
            }
        }

        private async void Actor(ResponderModel data)
        {
            bool handled = false;

            EventHandler<ActorEventArgs> actHandler = Act;

            if (actHandler != null)
            {
                ActorEventArgs args = new ActorEventArgs(this.mainContext);

                actHandler(this, args);

                handled = args.handled;
            }

            if (!handled)
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
                            authToken = GetAuthToken(); //check and make sure they're logged in.
                        }
                        catch (KeyNotFoundException keyErr)
                        {
                            // user is not logged in. redirect to login screen.
                            Debug.WriteLine(String.Format("Actor: {0}", keyErr.Message));
                            return;
                        }

                        headers = new Dictionary<string, string>();
                        headers.Add(Resources.PlexiResources.AuthTokenHeader, authToken);
                        headers.Add(Resources.PlexiResources.AuthDeviceHeader, GetDuid());
                    }

                    ActorModel response = await RequestHelper<ActorModel>(endpoint, "POST", mainContext, headers);

                    //ClearContext();

                    if (response.error != null)
                    {
                        switch (response.error.code)
                        {
                            case 401:
                                AuthorizationMessage(data.data.model);
                                break;
                            default:
                                ChangeState(State.Exception, response.error.msg);
                                break;
                        }
                    }
                    else
                    {
                        EventHandler<CompleteEventArgs> completeHandler = Complete;

                        if (completeHandler != null)
                        {
                            completeHandler(this, new CompleteEventArgs(response));
                        }
                    }
                }
                else
                {
                    // we have no actor, so send responder object to show
                    ShowDialog(data);
                }
            }
            
            ClearContext();
        }

        #region helpers
        private async Task<T> RequestHelper<T>(string endpoint, string method)
        {
            return await RequestHelper<T>(endpoint, method, null, null, false);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, object data)
        {
            return await RequestHelper<T>(endpoint, method, data, null, false);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, object data, Dictionary<string, string> headers)
        {
            return await RequestHelper<T>(endpoint, method, data, headers, false);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, object data, bool includeNulls)
        {
            return await RequestHelper<T>(endpoint, method, data, null, includeNulls);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, Dictionary<string, string> headers)
        {
            return await RequestHelper<T>(endpoint, method, null, headers, false);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, bool includeNulls)
        {
            return await RequestHelper<T>(endpoint, method, null, null, includeNulls);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, Dictionary<string, string> headers, bool includeNulls)
        {
            return await RequestHelper<T>(endpoint, method, null, headers, includeNulls);
        }

        private async Task<T> RequestHelper<T>(string endpoint, string method, object data, Dictionary<string, string> headers, bool includeNulls)
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

        public string GetDuid()
        {
            //return "cRljODI+F0i6w8l72x9Kc9Ez6V8=";
            
            
            byte[] duidAsBytes = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];

            return Convert.ToBase64String(duidAsBytes);
        }

        public string GetAuthToken()
        {
            //return "CF08o2kLQ2qbCVguyLgsTB71p4J2FGt2A79cKVWtW1eiiMxK5zkorrDw6GAyz4zo|1385589452|c23807e8adee2d5c22501e7d795992db54b4d392585f0fe7e4c7bf35bed9610a";

            
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
                            LocationModel currLocation =  await GetDeviceInfo();
                            payload["location"] = new Dictionary<string, object>()
                            {
                                { "timestamp", currLocation.timestamp },
                                { "timeoffset", currLocation.timeoffset.Hours },
                                { "latitude", currLocation.geoCoordinate.Latitude },
                                { "longitude", currLocation.geoCoordinate.Longitude }
                            };
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
                    foreach (Tuple<string, string> datetime in datetimes)
                    {
                        string first = datetime.Item1;
                        string second = datetime.Item2;

                        if (data.ContainsKey(datetime.Item1) || data.ContainsKey(datetime.Item2))
                        {
                            bool includeDate = true;
                            bool includeTime = true;

                            if (!data.ContainsKey(datetime.Item1))
                            {
                                //data[datetime.Item1] = null;
                                includeDate = false;
                            }

                            if (!data.ContainsKey(datetime.Item2))
                            {
                                //data[datetime.Item2] = null;
                                includeTime = false;
                            }

                            // perform replacement
                            if (data[datetime.Item1] != null || data[datetime.Item2] != null)
                            {
                                //Dictionary<string, string> build = Datetime.BuildDatetimeFromJson(data[datetime.Item1], data[datetime.Item2]);
                                Tuple<string, string> result = Datetime.DateTimeFromJson(data[first], data[second]);                               

                                Debug.WriteLine(String.Format("datetime build - {0} {1}", result.Item1, result.Item2));

                                if (includeDate)
                                    data[first] = result.Item1;

                                if (includeTime)
                                    data[second] = result.Item2;
                            }

                            // cleanup
                            /*
                            if (removeDate == true)
                            {
                                data.Remove(datetime.Item1);
                            }

                            if (removeTime == true)
                            {
                                data.Remove(datetime.Item2);
                            }
                             */
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("BuildDateTime Error: {0}", err.Message));
            }

            Debug.WriteLine("after build datetime");
            Debug.WriteLine(SerializeData(data));

            return data;
        }

        public async Task<LocationModel> GetDeviceInfo()
        {
            try
            {
                LocationModel geolocation = LocationService.Default.CurrentPosition;

                // geo fell down so 'manually' get geolocation
                if (geolocation == null)
                {
                    Debug.WriteLine("manually retrieve device location");
                    geolocation = await LocationService.Default.GetGeolocation();
                }

                geolocation.timestamp = Datetime.ConvertToUnixTimestamp(DateTime.Now);
                geolocation.timeoffset = DateTimeOffset.Now.Offset;

                return geolocation;
            }
            catch (Exception err)
            {
               
                Debug.WriteLine(err.Message);

                /*
                if ((uint)err.HResult == 0x80004004)
                {
                    // location has been diabled in phone settings. display appropriate message
                }
                else
                {
                    // unforeseen error
                }
                */

                return default(LocationModel);
            }
        }

        public async Task<List<AccountModel>> GetAccounts()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add(Resources.PlexiResources.AuthDeviceHeader, GetDuid());
            headers.Add(Resources.PlexiResources.AuthTokenHeader, GetAuthToken());

            return await RequestHelper<List<AccountModel>>(ACCOUNTS, "GET", headers);
        }

        public async void RemoveAccount(long id)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add(Resources.PlexiResources.AuthDeviceHeader, GetDuid());
            headers.Add(Resources.PlexiResources.AuthTokenHeader, GetAuthToken());

            await RequestHelper<Dictionary<string, object>>(ACCOUNT, "DELETE", headers);
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
