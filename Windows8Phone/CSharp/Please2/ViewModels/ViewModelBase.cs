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

using Microsoft.Phone.Controls;

//using Windows.Foundation;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight;

using Newtonsoft.Json;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

namespace Please2.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        // gets completed with all the necessary fields in order to fulfill an action
        ClassifierModel mainContext = null;

        // indicates fields that need to be completed in the main context
        ResponderModel tempContext = null;

        StateModel currentState = new StateModel();

        // stop potential api request looping. mostly applies to passive disambiguation
        int counter = 0;

        string initialQuery;

        public INavigationService navigationService;

        public ViewModelBase()
        {
            Debug.WriteLine("viewmodelbase constructor - " + this.GetType());

            // attach navigation service
            this.navigationService = SimpleIoc.Default.GetInstance<INavigationService>();

            navigationService.Navigated += OnNavigated;
        }

        protected void OnNavigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("on navigated - " + this.GetType());

            // listen for Please API state changes 
            currentState.PropertyChanged += OnStateChanged;

            navigationService.Navigating += OnNavigating;

            navigationService.Navigated -= OnNavigated;

            // listen for querys from ViewBase.cs
            Messenger.Default.Register<QueryMessage>(this, HandleUserInput);
        }

        // NOTE: very important to de-register events to prevent the buildup of event listeners
        protected void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("on navigating - " + this.GetType());

            navigationService.Navigated -= OnNavigated;

            navigationService.Navigating -= OnNavigating;

            currentState.PropertyChanged -= OnStateChanged;

            Debug.WriteLine("register query message");

            Messenger.Default.Unregister<QueryMessage>(this);
        }

        protected void HandleUserInput(QueryMessage message)
        {
            Debug.WriteLine("handle user input");
            
            HandleUserInput(message.Query);
        }

        public void HandleUserInput(string query)
        {
            initialQuery = query;

            currentState.Response = query;

            if (currentState.State == "inprogress")
            {
                currentState.State = "disambiguate:active";
            }
            else
            {
                currentState.State = "init";
            }
        }

        protected async void OnStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                        DisambiguatePersonal((ResponderModel)currentState.Response);
                        break;

                    case "disambiguate:active":
                        DisambiguateActive((string)currentState.Response);
                        break;

                    case "inprogress":
                        Show((ResponderModel)currentState.Response);
                        break;

                    case "restart":
                        await Auditor((ResponderModel)currentState.Response);
                        break;

                    case "completed":
                        Actor((ResponderModel)currentState.Response);
                        break;

                    case "error":
                        Show((ResponderModel)currentState.Response);
                        break;

                    case "exception":
                        ErrorMessage((string)currentState.Response);
                        break;
                }
            }
        }

        // NOTE: called from inprogress status
        protected void Show(ResponderModel response)
        {
            // since this overload only get's triggered on an "inprogress" status, check current page
            // if current page is not conversation.xaml, navigate to conversation.xaml
            // else pass data to overload below which will be picked up by the conversationViewModel
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
                    Debug.WriteLine("set conversation before navigation - " + this.GetType());

                    // set Conversation ViewModel data
                    if (!SimpleIoc.Default.IsRegistered<ConversationViewModel>())
                    {
                        Debug.WriteLine("register conversation viewmodel");
                        // need to set the data to a property. can't call a method
                        SimpleIoc.Default.Register<ConversationViewModel>();
                    }

                    // get instance of conversation view model and update dialoglist
                    //var cvm = SimpleIoc.Default.GetInstance<ConversationViewModel>();
                    
                    //cvm.AddDialog("user", initialQuery);
                    //cvm.AddDialog("please", (string)response.show.simple["text"]);

                    // navigate to conversation.xaml
                    //navigationService.NavigateTo(new Uri("/Views/Conversation.xaml", UriKind.Relative));
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Show:inprogress - " + err.Message);
            }
            /*
            string show = null;
            // parse and pass to Show
            if (response.show.simple.ContainsKey("text"))
            {
                show = (string)response.show.simple["text"];
            }

            //Show("please", response.speak, show);            
            try
            {
                conversationService.Show("please", response.speak, show);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
             */
        }

        // NOTE: called from Actor method
        protected virtual void Show(ShowModel showModel, string speak = "")
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
   
        protected void Show(string type, string speak = "", string show = null, string link = null)
        {
            Messenger.Default.Send(new ShowMessage(show, speak, link));
        }
     
        protected void ErrorMessage(string message)
        {
            // TODO: use dialogservice
            MessageBox.Show(message, "Server Error", MessageBoxButton.OK);
        }

        protected async Task Classify(string query)
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
                    await Auditor(classifierResults);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected async void DisambiguateActive(string data)
        {
            List<string> types = new List<string>();

            string field = tempContext.field;

            string type = tempContext.type;

            types.Add(type);

            string payload = data;

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.types = types;

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

        protected async void DisambiguatePassive(ResponderModel data)
        {
            List<string> types = new List<string>();

            string field = data.field;

            string type = data.type;

            types.Add(type);

            object payload;

            if (field.Contains("."))
            {
                payload = FindOrReplace(field);
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
            postData.types = types;
            postData.device_info = deviceInfo;

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/passive", "POST", postData);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        protected async void DisambiguatePersonal(ResponderModel data)
        {
            List<string> types = new List<string>();

            string field = data.field;

            string type = data.type;

            types.Add(type);

            object payload;

            if (field.Contains("."))
            {
                payload = FindOrReplace(field);
            }
            else
            {
                payload = mainContext.payload[field];
            }

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.types = types;

            Dictionary<string, object> response = await RequestHelper<Dictionary<string, object>>(AppResources.PudEndpoint + "disambiguate", "POST", postData);

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        protected async void DisambiguateResponseHandler(Dictionary<string, object> response, string field, string type)
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
                    DoClientOperations(response);

                    //response = ReplaceLocation(response);

                    response = BuildDateTime(response);

                    if (response.ContainsKey(type))
                    {
                        if (field.Contains("."))
                        {
                            FindOrReplace(field, response[type]);
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

                    await Auditor(request);
                }
            }
            else
            {
                Debug.WriteLine("oops no responder response");
            }
        }

        protected async Task Auditor(ResponderModel data)
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

        protected async Task Auditor(ClassifierModel data)
        {
            mainContext = data;

            Dictionary<string, object> request = new Dictionary<string, object>();

            request.Add("action", data.action);
            request.Add("model", data.model);
            request.Add("payload", data.payload);

            await Auditor(request);
        }

        protected async Task Auditor(Dictionary<string, object> data)
        {

            if (data.ContainsKey("payload"))
            {
                var payload = (Dictionary<string, object>)data["payload"];

                //data["payload"] = ReplaceLocation(payload);
                data["payload"] = BuildDateTime(payload);
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

                    if (state == "inprogress")
                        tempContext = response;

                    // create event and trigger based on status
                    currentState.Response = response;
                    currentState.State = state;
                }
            }
        }

        protected async void Actor(ResponderModel data)
        {
            string actor = data.actor;

            string endpoint;

            if (actor != null)
            {
                if (actor.Contains("private:"))
                {
                    endpoint = AppResources.PudEndpoint + "actors/" + actor.Replace("private:", "");
                }
                else
                {
                    endpoint = AppResources.ResponderEndpoint + "actors/" + actor;

                    ActorModel response = await RequestHelper<ActorModel>(endpoint, "POST", mainContext);

                    if (response.error != null)
                    {
                        currentState.Response = response.error.msg;
                        currentState.State = "exception";
                    }
                    else
                    {
                        //Show(response.show, response.speak);
                        ActorResponseHandler(actor, response);
                    }
                }
            }
            else
            {
                // we have no actor, so send show data to ViewBase.cs
                Show(data.show);
            }

            return;
        }

         protected void ActorResponseHandler(string actor, ActorModel response)
        {
            if (response.show.structured != null && response.show.structured.ContainsKey("template"))
            {
                Dictionary<string, object> structured = response.show.structured;
                string[] template = (structured["template"] as string).Split(':');

                var type = template[0];
                var templateName = template[1];
                string page = "/Views/";

                // TODO: consider making template type to page name a resource file (.resx)
                switch (type)
                {
                    case "list":
                        page += "ListResultsPage.xaml";
                        
                        if (!SimpleIoc.Default.ContainsCreated<ListViewModel>(page))
                        {                            
                            SimpleIoc.Default.Register(
                                () => new ListViewModel
                                {
                                    //TemplateName = templateName,
                                    ListResults = (List<object>)structured["items"]
                                },
                                page);
                        }
                        break;

                    default:
                        page += "ConversationPage.xaml";
                        /*
                        if (!SimpleIoc.Default.ContainsCreated<ConversationViewModel>(page))
                        {
                            SimpleIoc.Default.Register(
                                () => new ConversationViewModel(),                        
                                page);
                        }
                        */
                        break;
                }

                navigationService.NavigateTo(new Uri(page, UriKind.Relative));
            }
        }

        #region helpers
        protected async Task<T> RequestHelper<T>(string endpoint, string method, object data = null)
        {
            var req = new Request();

            req.Method = method;

            Debug.WriteLine(endpoint);
            // TODO: send message to view to activate progress indicator
            //SystemTray.ProgressIndicator.IsVisible = true;
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

            // TODO: send message to view to deactivate progress indicator
            //SystemTray.ProgressIndicator.IsVisible = false;
            Messenger.Default.Send(new ProgressMessage(false));

            return response;
        }

        protected Dictionary<string, object> DoClientOperations(Dictionary<string, object> response)
        {
            //response = ReplaceLocation(response);
            response = BuildDateTime(response);
            response = PrependTo(response);
          
            // from auditor
            // data["payload"] = ReplaceLocation(payload);
            // data["payload"] = BuildDateTime(payload);

            return response;
        }

        protected Dictionary<string, object> PrependTo(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("unused_tokens"))
            {
                return data;
            }

            var prepend = (string)((Newtonsoft.Json.Linq.JArray)data["unused_tokens"]).Aggregate( (i, j) => i + " " + j );

            var field = (string)data["prepend_to"];

            var payloadField = "";
            
            if (mainContext.payload.ContainsKey(field) && mainContext.payload[field] != null)
            {
                payloadField = " " + (string)mainContext.payload[field];
            }

            mainContext.payload[field] = prepend + payloadField;

            return data;
        }

        /*
        protected void Replace(string field, object type)
        {
            var fields = field.Split('.').ToList();


        }
        */

        /*
        protected void Find(string field)
        {
            var fields = field.Split('.').ToList();

            var myTest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>("{\"action\":\"create\",\"model\":\"hotel_booking\",\"payload\":{\"duration\":null,\"start_date\":null,\"location\":null}}");

            var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(SerializeData(myTest));

            var stackToVisit = new Stack<Newtonsoft.Json.Linq.JObject>();

            stackToVisit.Push(jObject);

            // keep visiting iterating until the stack of dictionaries to visit is empty
            while (stackToVisit.Count > 0)
            {
                var next = stackToVisit.Pop();

                var res = fields.Aggregate((a, b) =>
                {
                    return 
                });
            }

            Debug.WriteLine(res);

        }
        */

        // if type is null, it's a find. 
        // if type has a value, it's a replace
        // TODO: REFACTOR THIS POS. SPLIT FIND AND REPLACE INTO TWO SEPERATE METHODS
        protected object FindOrReplace(string field, object type = null)
        {
            try
            {
                Debug.WriteLine("find or replace");
                Debug.WriteLine(SerializeData(mainContext));

                var fields = field.Split('.').ToList();

                // turn our context into a jobject we can work with
                var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(SerializeData(mainContext));
                // SingleForecast = ((Newtonsoft.Json.Linq.JToken)showModel.structured["item"]).ToObject<WeatherModel>();
                //var jObject = mainContext;


                var stackToVisit = new Stack<Newtonsoft.Json.Linq.JObject>();

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
                                nextObject.Add(fields.First(), Newtonsoft.Json.Linq.JToken.FromObject(type));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < nextObject.Count; i++)
                            {
                                var keyValuePair = nextObject.ElementAt<KeyValuePair<string, Newtonsoft.Json.Linq.JToken>>(i);

                                if (fields.Count() > 1 && keyValuePair.Key == fields.First())
                                {
                                    fields.RemoveAt(0);
                                    stackToVisit.Push(keyValuePair.Value as Newtonsoft.Json.Linq.JObject);
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
                                            nextObject[keyValuePair.Key] = Newtonsoft.Json.Linq.JToken.FromObject(type);
                                        }
                                        catch (Exception err)
                                        {
                                            Debug.WriteLine(err.Message);
                                        }
                                    }
                                }
                                else if (nextObject[fields.First()] == null && type != null)
                                {
                                    nextObject.Add(fields.First(), Newtonsoft.Json.Linq.JToken.FromObject(type));
                                }
                            }
                        }
                    }
                }

                // turn our jobject back into the proper context model
                if (type != null)
                {
                    mainContext = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassifierModel>(SerializeData(jObject));
                }

                return null;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return null;
            }
        }

        protected Dictionary<string, object> ReplaceLocation(Dictionary<string, object> payload)
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
                            payload["location"] = GetDeviceInfo();
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

        protected Dictionary<string, object> BuildDateTime(Dictionary<string, object> data)
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

                            /*
                            if (data.ContainsKey(datetime.Item1))
                            {
                                if (data[datetime.Item1] != null)
                                {
                                    if (data[datetime.Item1].GetType() == typeof(string))
                                        data[datetime.Item1] = (string)data[datetime.Item1];

                                    if (data[datetime.Item1].GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                                        data[datetime.Item1] = (Newtonsoft.Json.Linq.JObject)data[datetime.Item1];
                                }
                            }
                            else
                            {
                                data[datetime.Item1] = null;
                                removeDate = true;
                            }

                            if (data.ContainsKey(datetime.Item2))
                            {
                                if (data[datetime.Item2] != null)
                                {
                                    if (data[datetime.Item2].GetType() == typeof(string))
                                        data[datetime.Item2] = (string)data[datetime.Item2];

                                    if (data[datetime.Item2].GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                                        data[datetime.Item2] = (Newtonsoft.Json.Linq.JObject)data[datetime.Item2];
                                }
                            }
                            else
                            {
                                data[datetime.Item2] = null;
                                removeTime = true;
                            }
                            */

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

        protected async Task<Dictionary<string, object>> GetDeviceInfo()
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

        protected string SerializeData(object data)
        {
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Include;
            jsonSettings.NullValueHandling = NullValueHandling.Include;

            return Newtonsoft.Json.JsonConvert.SerializeObject(data, jsonSettings);
        }

        protected static string ProfanityFilter(Match match)
        {
            string replacement = "";

            for (var i = 1; i < match.Groups.Count; i++)
            {
                for (var j = 0; j < match.Groups[i].Length; j++)
                {
                    replacement += "*";
                }
            }

            return replacement;
        }
        #endregion
    }    
}
