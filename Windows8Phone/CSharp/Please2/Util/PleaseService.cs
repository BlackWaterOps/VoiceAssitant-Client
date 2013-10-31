using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Resources;
using Please2.ViewModels;

namespace Please2.Util
{
    public class PleaseService : IPleaseService
    {
        // used by auditor
        private string[] auditorStates = new string[] { "inprogress", "choice" };

        // used by BuildDateTime
        private List<Tuple<string, string>> datetimes = new List<Tuple<string, string>>();

        // used in RequestHelper to benchmark request times
        private Stopwatch stopWatch;

        // gets completed with all the necessary fields in order to fulfill an action
        private ClassifierModel mainContext = null;

        // indicates fields that need to be completed in the main context
        private ResponderModel tempContext = null;

        private StateModel currentState = new StateModel();

        private INavigationService navigationService { get; set; }

        private DispatcherTimer contextTimer;

        public string OriginalQuery { get; private set; }

        public PleaseService()
        {
            // attach navigation service
            this.navigationService = ViewModelLocator.GetViewModelInstance<INavigationService>();
            
            // listen for Please API state changes 
            this.currentState.PropertyChanged += OnStateChanged;

            CreateTimer();

            if (this.stopWatch == null)
            {
                this.stopWatch = new Stopwatch();
            }
        }

        public void Query(string query)
        {
            this.OriginalQuery = query;

            string newState;

            if (currentState.State == "inprogress" || currentState.State == "error")
            {
                newState = "disambiguate:active";
            }
            else if (currentState.State == "choice")
            {
                newState = "disambiguate:candidate";
            }
            else
            {
                newState = "init";
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
            /*
            this.currentState = new StateModel();
            currentState.PropertyChanged += OnStateChanged;
            */
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
                        Show((ResponderModel)currentState.Response);
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
            Debug.WriteLine("please service reset");
            ClearContext();

            ConversationViewModel vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();
            vm.ClearDialog();

            navigationService.NavigateTo(ViewModelLocator.MainMenuPageUri);
        }

        private void ChoiceList(ResponderModel response)
        {
            try
            {
                Dictionary<string, object> simple = response.show.simple;

                if (simple.ContainsKey("list"))
                {
                    ResourceDictionary templates = ViewModelLocator.ListTemplates;

                    List<ChoiceModel> list = ((JArray)simple["list"]).ToObject<List<ChoiceModel>>();

                    ListViewModel vm = ViewModelLocator.GetViewModelInstance<ListViewModel>();

                    vm.ListResults = list.ToList<object>();
                    vm.Template = (DataTemplate)templates["choice"];
                    vm.Title = (string)simple["text"];

                    Show(response.show, response.speak);

                    navigationService.NavigateTo(ViewModelLocator.ListResultsPageUri);
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
        private void Show(ResponderModel response)
        {
            try
            {
                PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (frame.CurrentSource.Equals(ViewModelLocator.ConversationPageUri))
                {
                    Show(response.show, response.speak);
                }
                else
                {
                    //ConversationViewModel vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();
                    
                    // this won't have any speak
                    //vm.AddDialog("please", (string)response.show.simple["text"]);

                    Show(response.show, response.speak);

                    // navigate to conversation.xaml
                    this.navigationService.NavigateTo(ViewModelLocator.ConversationPageUri);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Show:inprogress - " + err.Message);
            }
        }

        // NOTE: called from Actor method
        private void Show(ShowModel showModel, string speak = "")
        {
            if (showModel.simple.ContainsKey("text"))
            {
                string show = (string)showModel.simple["text"];

                string link = null;

                if (showModel.simple.ContainsKey("link"))
                    link = (string)showModel.simple["link"];

                Show("please", speak, show, link);
            }
        }
        
        private void Show(string type, string speak = "", string show = null, string link = null)
        {
            Messenger.Default.Send(new ShowMessage(show, speak, link));
        }
     
        private void ErrorMessage(string message)
        {
            // pass message to view
            MessageBoxResult response = MessageBox.Show(message, "Server Error", MessageBoxButton.OK);

            if (response == MessageBoxResult.OK)
            {
                ClearContext();
            }
        }

        private async Task Classify(string query)
        {
            try
            {
                ClassifierModel classifierResults = await RequestHelper<ClassifierModel>((AppResources.ClassifierEndpoint + "?query=" + query), "GET");

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
                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/active", "POST", postData, true);

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
                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/candidate", "POST", postData, true);

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
                MessageBox.Show(err.Message);
                return;
            }

            DisambiguatorModel postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.device_info = deviceInfo;

            Debug.WriteLine("disambig passive");
            Debug.WriteLine(SerializeData(postData, true));

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/passive", "POST", postData, true);

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

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.PudEndpoint + "disambiguate", "POST", postData);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        private async void DisambiguateResponseHandler(Dictionary<string, object> response, string field, string type)
        {
            //Debug.WriteLine("disambig handler");
            //Debug.WriteLine(SerializeData(response));

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

                ResponderModel response = await RequestHelper<ResponderModel>(AppResources.ResponderEndpoint + "audit", "POST", context);

                if (response.error != null)
                {
                    this.currentState.Set("exception", response.error.msg);
                }
                else
                {
                    string state = response.status.Replace(" ", "");

                    if (this.auditorStates.Contains(state))
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
            ConversationViewModel vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();

            vm.AddDialog("user", choice.text);

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
                string endpoint = AppResources.ResponderEndpoint + "actors/" + actor;

                if (actor.Contains("private:"))
                {
                    endpoint = AppResources.PudEndpoint + "actors/" + actor.Replace("private:", "");
                }

                ActorModel response = await RequestHelper<ActorModel>(endpoint, "POST", mainContext);

                ClearContext();
                
                if (response.error != null)
                {
                    this.currentState.Set("exception", response.error.msg);
                }
                else
                {
                    Debug.WriteLine("Actor response handler");
                    Show(response.show, response.speak);
                    ActorResponseHandler(actor, response);
                }
            }
            else
            {
                // we have no actor, so send responder object to show
                Show(data);
            }

            return;
        }

        private Dictionary<string, object> PopulateViewModel(string templateName, Dictionary<string, object> structured)
        {            
            try
            {
                ViewModelLocator locator = App.Current.Resources["Locator"] as ViewModelLocator;

                PropertyInfo viewmodelProperty = locator.GetType().GetProperty(templateName.CamelCase() + "ViewModel");

                if (viewmodelProperty == null)
                {
                    Debug.WriteLine("pouplateviewmodel: view model " + templateName + " could not be found");
                    return null;
                }

                object viewModel = viewmodelProperty.GetValue(locator, null);

                MethodInfo populateMethod = viewModel.GetType().GetMethod("Populate");

                if (populateMethod == null)
                {
                    Debug.WriteLine("populateviewmodel: 'Populate' method not implemented in " + templateName);
                    return null;
                }

                if (structured.ContainsKey("items") && ((JArray)structured["items"]).Count <= 0)
                {
                    Debug.WriteLine("populateviewmodel: items list is emtpy nothing to set");
                    return null;
                }

                if (structured.ContainsKey("item") && ((JObject)structured["item"]).Count <= 0)
                {
                    Debug.WriteLine("populateviewmodel: item object is emtpy nothing to set");
                    return null;
                }

                object[] parameters = (templateName == "list") ? new object[] { structured } : new object[] { templateName, structured };

                return (Dictionary<string, object>)populateMethod.Invoke(viewModel, parameters);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return null;
            }
        }

        private Uri LoadSingleResult(Dictionary<string, object> structured)
        {
            ResourceDictionary templates = ViewModelLocator.SingleTemplates;

            Uri page = ViewModelLocator.SingleResultPageUri;

            string[] template = (structured["template"] as string).Split(':');

            string type = template[0];
            string templateName = template[1];

            if (template.Count() > 2)
            {
                templateName += ":" + template[2];
            }

            if (templates[templateName] == null)
            {
                Debug.WriteLine("single template not found: " + templateName);
                return null;
            }

            SingleViewModel singleViewModel = ViewModelLocator.GetViewModelInstance<SingleViewModel>();

            singleViewModel.ContentTemplate = (DataTemplate)templates[templateName];

            Dictionary<string, object> data = PopulateViewModel(template[1], structured);

            if (data == null)
            {
                return null;
            }

            if (data.ContainsKey("page"))
            {
                page = (Uri)data["page"];
            }

            singleViewModel.Title = null;

            if (data.ContainsKey("title"))
            {
                singleViewModel.Title = (string)data["title"];
            }
           
            singleViewModel.SubTitle = null;

            if (data.ContainsKey("subtitle"))
            {
                singleViewModel.SubTitle = (string)data["subtitle"];
            }

            return page;
        }

        private void ActorResponseHandler(string actor, ActorModel response)
        {
            Uri page = ViewModelLocator.ConversationPageUri;

            if (response.show.structured != null && response.show.structured.ContainsKey("template"))
            {
                Dictionary<string, object> structured = response.show.structured;

                string[] template = (structured["template"] as string).Split(':');

                string type = template[0];
                string templateName = null;

                if (template.Count() > 1)
                {
                    templateName = template[1];
                }

                if (templateName != null)
                {
                    Dictionary<string, object> data;

                    ResourceDictionary templates;

                    Uri uri;

                    switch (type)
                    {
                        case "list":
                            templates = ViewModelLocator.ListTemplates;

                            switch (templateName)
                            {
                                // override. list items that act like single items
                                case "news":
                                    uri = LoadSingleResult(structured);

                                    if (uri != null)
                                    {
                                        page = uri;
                                    }
                                    break;

                                default:
                                    data = PopulateViewModel("list", structured);

                                    if (data != null)
                                    {
                                        page = ViewModelLocator.ListResultsPageUri;
                                    }
                                    break;
                            }
                            break;

                        case "simple":
                        case "single":
                            uri = LoadSingleResult(structured);

                            if (uri != null)
                            {
                                page = uri;
                            }
                            break;
                    }
                }
            }

            navigationService.NavigateTo(page);
        }

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

        #region helpers
        private async Task<T> RequestHelper<T>(string endpoint, string method, object data = null, bool includeNulls = false)
        {
            Request req = new Request();

            req.Method = method;

            // Notify the view that the request has begun
            Messenger.Default.Send(new ProgressMessage(true));

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

            Debug.WriteLine("this request took " + stopWatch.Elapsed + " to complete");

            this.stopWatch.Reset();

            // Notify the view that the request is complete
            Messenger.Default.Send(new ProgressMessage(false));

            return response;
        }

        // Note: 
	    // data represents the payload object in response to classification
	    // and represents the entire response object in response to disambiguation
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

        private async Task<Dictionary<string, object>> GetDeviceInfo()
        {
            Dictionary<string, object> deviceInfo = new Dictionary<string, object>()
            {
                {"latitude", 33.4930947},
                {"longitude", -111.928558}
            };

            /*
            deviceInfo["timestamp"] = Please2.Util.Datetime.ConvertToUnixTimestamp(DateTime.Now);
            deviceInfo["timeoffset"] = DateTimeOffset.Now.Offset.Hours;

            Dictionary<string, object> geolocation = Please2.Util.Location.CurrentPosition;
            // geo fell down so 'manually' get geolocation
            if (geolocation == null)
            {
                geolocation = await Please2.Util.Location.GetGeolocation();
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
            */

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
