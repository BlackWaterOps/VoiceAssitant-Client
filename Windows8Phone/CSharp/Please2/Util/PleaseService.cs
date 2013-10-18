using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        // gets completed with all the necessary fields in order to fulfill an action
        private ClassifierModel mainContext = null;

        // indicates fields that need to be completed in the main context
        private ResponderModel tempContext = null;

        private StateModel currentState = new StateModel();

        private int counter = 0;

        private int testCounter = 0;

        private INavigationService navigationService { get; set; }

        private DispatcherTimer contextTimer;

        public string OriginalQuery { get; private set; }

        public PleaseService()
        {
            Debug.WriteLine(testCounter);

            testCounter++;

            // attach navigation service
            this.navigationService = ViewModelLocator.GetViewModelInstance<INavigationService>();
            
            // listen for Please API state changes 
            currentState.PropertyChanged += OnStateChanged;

            CreateTimer();
        }

        public void Query(string query)
        {
            OriginalQuery = query;

            currentState.Response = query;

            if (currentState.State == "inprogress" || currentState.State == "error")
            {
                currentState.State = "disambiguate:active";
            }
            else if (currentState.State == "choice")
            {
                currentState.State = "disambiguate:candidate";
            }
            else
            {
                currentState.State = "init";
            }
        }

        public void ResetTimer()
        {
            contextTimer.Stop();
            CreateTimer();
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

                    case "disambiguate":
                        DisambiguatePassive((ResponderModel)currentState.Response);
                        break;

                    case "disambiguate:personal":
                        ActorInterceptor(mainContext.model);

                        //DisambiguatePersonal((ResponderModel)currentState.Response);
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
                        await Auditor((ResponderModel)currentState.Response);
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
            contextTimer = new DispatcherTimer();
            contextTimer.Interval = TimeSpan.FromMinutes(2);
            contextTimer.Tick += Reset;
        }

        private void ClearContext()
        {
            mainContext = null;
            tempContext = null;
            currentState = new StateModel();

            ResetTimer();
        }

        private void Reset(object sender, EventArgs e)
        {
            ClearContext();

            var vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();
            vm.DialogList = null;

            navigationService.NavigateTo(ViewModelLocator.MainMenuPageUri);
        }

        private void ChoiceList(ResponderModel response)
        {
            try
            {
                var simple = response.show.simple;

                if (simple.ContainsKey("list"))
                {
                    var templates = ViewModelLocator.ListTemplates;

                    var list = ((JArray)simple["list"]).ToObject<List<ChoiceModel>>();

                    var vm = ViewModelLocator.GetViewModelInstance<ListViewModel>();

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
                var frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (frame.CurrentSource.ToString().Contains("Conversation.xaml"))
                {
                    Debug.WriteLine("pass data to show override");
                    Show(response.show, response.speak);
                }
                else
                {
                    var vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();
                    
                    vm.AddDialog("please", (string)response.show.simple["text"]);

                    // navigate to conversation.xaml
                    navigationService.NavigateTo(ViewModelLocator.ConversationPageUri);
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
            var response = MessageBox.Show(message, "Server Error", MessageBoxButton.OK);

            if (response == MessageBoxResult.OK)
            {
                ClearContext();
            }
        }

        private async Task Classify(string query)
        {
            try
            {
                var classifierResults = await RequestHelper<ClassifierModel>((AppResources.ClassifierEndpoint + "?query=" + query), "GET");

                if (classifierResults.error != null)
                {
                    currentState.Response = classifierResults.error.message;
                    currentState.State = "exception";
                }
                else
                {
                    // should really trigger a state change here instead of calling method directly
                    await Auditor(classifierResults);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private async void DisambiguateActive(string data)
        {
            List<string> types = new List<string>();

            string field = tempContext.field;

            string type = tempContext.type;

            types.Add(type);

            string payload = data;

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;

            try
            {
                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/active", "POST", postData);

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
            var simple = tempContext.show.simple;

            string field = tempContext.field;

            string type = tempContext.type;

            var list = (simple.ContainsKey("list")) ? (JArray)simple["list"] : new JArray();

            string payload = data;

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.candidates = list;
          
            try
            {
                Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/candidate", "POST", postData);

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
                // payload = FindOrReplace(field);
                payload = Find(field);
            }
            else
            {
                payload = mainContext.payload[field];
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

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.device_info = deviceInfo;

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/passive", "POST", postData);

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
                //payload = FindOrReplace(field);
                payload = Find(field);
            }
            else
            {
                payload = mainContext.payload[field];
            }

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.PudEndpoint + "disambiguate", "POST", postData);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        private async void DisambiguateResponseHandler(Dictionary<string, object> response, string field, string type)
        {
            if (response != null)
            {
                if (response.ContainsKey("error"))
                {
                    var er = (ErrorModel)response["error"];

                    currentState.Response = er.message;
                    currentState.State = "exception";
                }
                else
                {
                    response = await DoClientOperations(response);

                    if (response.ContainsKey(type))
                    {
                        if (field.Contains("."))
                        {
                            //FindOrReplace(field, response[type]);
                            Replace(field, response[type]);
                        }
                        else
                        {
                            mainContext.payload[field] = response[type];
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Disambiguation response is missing type");
                    }

                    // clone mainContext so we don't pollute with unused_tokens
                    Dictionary<string, object> request = new Dictionary<string, object>();

                    request.Add("action", mainContext.action);
                    request.Add("model", mainContext.model);
                    request.Add("payload", mainContext.payload);

                    if (response.ContainsKey("unused_tokens"))
                    {
                        request.Add("unused_tokens", response["unused_tokens"]);
                    }

                    Debug.WriteLine("Disambiguate Response");
                    Debug.WriteLine(SerializeData(request));

                    await Auditor(request);
                }
            }
            else
            {
                Debug.WriteLine("oops no responder response");
            }
        }

        public async Task Auditor(ChoiceModel choice)
        {
            var vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();

            vm.AddDialog("user", choice.text);

            var field = tempContext.field;

            var t = choice.data;

            if (field.Contains("."))
            {
                Replace(field, choice.data);
            }
            else
            {
                mainContext.payload[field] = choice.data;
            }

            await Auditor(mainContext);
        }

        private async Task Auditor(ResponderModel data)
        {
            // reset counter
            counter = 0;

            if (data.data == null)
            {
                Debug.WriteLine("missing new replacement context");
                return;
            }

            await Auditor(data.data);
        }

        private async Task Auditor(ClassifierModel data)
        {
            mainContext = data;

            Dictionary<string, object> request = new Dictionary<string, object>();

            request.Add("action", data.action);
            request.Add("model", data.model);
            request.Add("payload", data.payload);

            await Auditor(request);
        }

        private async Task Auditor(Dictionary<string, object> data)
        {

            if (data.ContainsKey("payload"))
            {
                var payload = (Dictionary<string, object>)data["payload"];
                data["payload"] = await DoClientOperations(payload);
            }

            counter++;

            if (counter < 3)
            {
                ResponderModel response = await RequestHelper<ResponderModel>(AppResources.ResponderEndpoint + "audit", "POST", mainContext);

                if (response.error != null)
                {
                    currentState.Response = response.error.msg;
                    currentState.State = "exception";
                }
                else
                {
                    string state = response.status.Replace(" ", "");

                    if (state == "inprogress" || state == "choice")
                    {
                        tempContext = response;
                        contextTimer.Start();
                    }

                    // create event and trigger based on status
                    currentState.Response = response;
                    currentState.State = state;
                }
            }
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
                    //Debug.WriteLine("Actor exception");
                    currentState.Response = response.error.msg;
                    currentState.State = "exception";
                }
                else
                {
                    //Debug.WriteLine("Actor response handler");
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
            var ret = new Dictionary<string, object>();
            
            var locator = App.Current.Resources["Locator"] as ViewModelLocator;

            try
            {
                var viewmodelProperty = locator.GetType().GetProperty(templateName.CamelCase() + "ViewModel");

                if (viewmodelProperty != null)
                {
                    var viewModel = viewmodelProperty.GetValue(locator, null);

                    var populateMethod = viewModel.GetType().GetMethod("Populate");

                    if (populateMethod != null)
                    {
                        var parameters = (templateName == "list") ? new object[] { structured } : new object[] { templateName, structured};

                        return (Dictionary<string, object>)populateMethod.Invoke(viewModel, parameters);
                    }
                    else
                    {
                        Debug.WriteLine("reflection: 'Populate' method not implemented in " + templateName);
                    }
                }
                else
                {
                    Debug.WriteLine("reflection: view model " + templateName + " could not be found");
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }

            return ret;
        }

        private Uri LoadSingleResult(string templateName, Dictionary<string, object> structured)
        {
            var templates = ViewModelLocator.SingleTemplates;

            var page = ViewModelLocator.SingleResultPageUri;

            if (templates[templateName] != null)
            {
                var singleViewModel = ViewModelLocator.GetViewModelInstance<SingleViewModel>();

                singleViewModel.ContentTemplate = (DataTemplate)templates[templateName];

                var data = PopulateViewModel(templateName, structured);

                if (data.ContainsKey("page"))
                {
                    page = (Uri)data["page"];
                }

                if (data.ContainsKey("title"))
                {
                    singleViewModel.Title = (string)data["title"];
                }

                if (data.ContainsKey("subtitle"))
                {
                    singleViewModel.SubTitle = (string)data["subtitle"];
                }
            }
            else
            {
                Debug.WriteLine("single template not found: " + templateName);
                return null;
            }

            return page;
        }

        private void ActorResponseHandler(string actor, ActorModel response)
        {
            if (response.show.structured != null && response.show.structured.ContainsKey("template"))
            {
                Dictionary<string, object> structured = response.show.structured;

                string[] template = (structured["template"] as string).Split(':');

                var type = template[0];
                var templateName = template[1];

                Dictionary<string, object> data;

                ResourceDictionary templates;

                //SingleViewModel singleViewModel;

                Uri page = ViewModelLocator.ConversationPageUri;

                Uri uri;

                switch (type)
                {
                    case "list":
                        templates = ViewModelLocator.ListTemplates;

                        switch (templateName)
                        {
                            // override. list items that act like single items
                            case "news": 
                                uri = LoadSingleResult(templateName, structured);

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
                        // !!create a method for this logic!!
                        uri = LoadSingleResult(templateName, structured);

                        if (uri != null)
                        {
                            page = uri;
                        }
                        break;
                }

                navigationService.NavigateTo(page);
            }
        }

        // these are actor's that will be handled locally. no need to run out to web service
        // all the data needed to fulfill the tasks should be in the mainContext var.
        private void ActorInterceptor(string actor)
        {
            var payload = mainContext.payload;

            if (!SimpleIoc.Default.IsRegistered<ITaskService>())
            {
                SimpleIoc.Default.Register<ITaskService, TaskService>();
            }

            var tasks = SimpleIoc.Default.GetInstance<ITaskService>();

            tasks.MainContext = mainContext;

            switch (actor)
            {
                case "time": 
                    tasks.ShowClock();
                    break;

                case "email":
                case "email_denial":
                    tasks.ComposeEmail();
                    break;

                case "sms":
                case "sms_denial":
                    tasks.ComposeSms();
                    break;

                case "directions":
                case "directions_denial":
                    tasks.GetDirections();
                    break;

                case "call":
                case "call_denial":
                    tasks.PhoneCall();
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
        private async Task<T> RequestHelper<T>(string endpoint, string method, object data = null)
        {
            var req = new Request();

            req.Method = method;

            // Notify the view that the request has begun
            Messenger.Default.Send(new ProgressMessage(true));

            T response;

            if (method.ToLower() == "get")
            {
                response = await req.DoRequestJsonAsync<T>(endpoint);
            }
            else
            {
                req.ContentType = "application/json";

                response = await req.DoRequestJsonAsync<T>(endpoint, SerializeData(data));
            }
            
            // Notify the view that the request is complete
            Messenger.Default.Send(new ProgressMessage(false));

            return response;
        }

        // Note: 
	    // data represents the payload object in response to classification
	    // and represents the entire response object in response to disambiguation
        private async Task<Dictionary<string, object>> DoClientOperations(Dictionary<string, object> response)
        {
            response = await ReplaceLocation(response);
            response = BuildDateTime(response);
            response = PrependTo(response);

            return response;
        }

        private Dictionary<string, object> PrependTo(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("unused_tokens"))
            {
                return data;
            }

            var prepend = (string)((JArray)data["unused_tokens"]).Aggregate((i, j) => i + " " + j);

            var field = (string)data["prepend_to"];

            var payloadField = "";

            if (mainContext.payload.ContainsKey(field) && mainContext.payload[field] != null)
            {
                payloadField = " " + (string)mainContext.payload[field];
            }

            mainContext.payload[field] = prepend + payloadField;

            return data;
        }

        protected void Replace(string field, object type)
        {
            var fields = field.Split('.').ToList();

            var last = fields[fields.Count - 1];

            // convert to generic object
            var context = JsonConvert.DeserializeObject<object>(SerializeData(this.mainContext));

            var t = fields.Aggregate(context, (a, b) =>
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
            this.mainContext = JsonConvert.DeserializeObject<ClassifierModel>(SerializeData(context));
        }

        protected object Find(string field)
        {
            var fields = field.Split('.').ToList();

            // convert to generic object
            var context = JsonConvert.DeserializeObject<object>(SerializeData(this.mainContext));

            return fields.Aggregate(context, (a, b) => ((JObject)a)[b]);

            /*
            return fields.Aggregate(context, (a, b) =>
            {
                if (a.GetType() == typeof(JObject))
                {
                    Debug.WriteLine("Find A");
                    return ((JObject)a)[b];
                }
                else
                {
                    Debug.WriteLine("Find B");
                    return b;
                }
            });
             */
        }

        // if type is null, it's a find. 
        // if type has a value, it's a replace
        // TODO: REFACTOR THIS POS. SPLIT FIND AND REPLACE INTO TWO SEPERATE METHODS
        /*
        private object FindOrReplace(string field, object type = null)
        {
            try
            {
                Debug.WriteLine("find or replace");
                Debug.WriteLine(SerializeData(mainContext));

                var fields = field.Split('.').ToList();

                // turn our context into a jobject we can work with
                var jObject = JsonConvert.DeserializeObject<JObject>(SerializeData(mainContext));

                var stackToVisit = new Stack<JObject>();

                stackToVisit.Push(jObject);

                // keep visiting iterating until the stack of dictionaries to visit is empty
                while (stackToVisit.Count > 0)
                {
                    var nextObject = stackToVisit.Pop();

                    if (nextObject != null)
                    {
                        if (nextObject.Count == 0)
                        {

                            if (type != null)
                            {
                                nextObject.Add(fields.First(), JToken.FromObject(type));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < nextObject.Count; i++)
                            {
                                var keyValuePair = nextObject.ElementAt<KeyValuePair<string, JToken>>(i);

                                if (fields.Count() > 1 && keyValuePair.Key == fields.First())
                                {
                                    fields.RemoveAt(0);
                                    stackToVisit.Push(keyValuePair.Value as JObject);
                                }
                                else if (keyValuePair.Key == fields.First())
                                {
                                    if (type == null)
                                    {
                                        return (object)nextObject[keyValuePair.Key];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            nextObject[keyValuePair.Key] = JToken.FromObject(type);
                                        }
                                        catch (Exception err)
                                        {
                                            Debug.WriteLine(err.Message);
                                        }
                                    }
                                }
                                else if (nextObject[fields.First()] == null && type != null)
                                {
                                    nextObject.Add(fields.First(), JToken.FromObject(type));
                                }
                            }
                        }
                    }
                }

                // turn our jobject back into the proper context model
                if (type != null)
                {
                    mainContext = JsonConvert.DeserializeObject<ClassifierModel>(SerializeData(jObject));
                }

                return null;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return null;
            }
        }
        */
        private async Task<Dictionary<string, object>> ReplaceLocation(Dictionary<string, object> payload)
        {
            try
            {
                if (payload.ContainsKey("location") && payload["location"] != null)
                {
                    if (payload["location"].GetType() == typeof(string))
                    {
                        var location = (string)payload["location"];
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
            Debug.WriteLine(SerializeData(data));

            try
            {
                if (data != null)
                {
                    List<Tuple<string, string>> datetimes = new List<Tuple<string, string>>();

                    datetimes.Add(new Tuple<string, string>("date", "time"));
                    datetimes.Add(new Tuple<string, string>("start_date", "start_time"));
                    datetimes.Add(new Tuple<string, string>("end_date", "end_time"));

                    foreach (var datetime in datetimes)
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
                                data.Remove(datetime.Item1);

                            if (removeTime == true)
                                data.Remove(datetime.Item2);
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
                {"latitude", ""},
                {"longitude", ""}
            };

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

            return deviceInfo;
        }

        private string SerializeData(object data)
        {
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Include;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            return JsonConvert.SerializeObject(data, jsonSettings);
        }
        #endregion
    }
}
