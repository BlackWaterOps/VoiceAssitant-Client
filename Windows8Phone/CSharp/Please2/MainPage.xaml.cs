using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Input;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

using Please2.Models;
using Please2.Resources;
using Please2.Util;
using Please2.ViewModels;

using Newtonsoft.Json;


// TODO: rename mainContext to requestState

namespace Please2
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string PLACEHOLDER_TEXT = "eg. Book a Hotel";

        // gets completed with all the necessary fields in order to fulfill an action
        ClassifierModel mainContext = null;

        // indicates fields that need to be completed in the main context
        ResponderModel tempContext = null;

        // Speech handling
        SpeechSynthesizer synthesizer;
        
        SpeechRecognizerUI recognizer;

        IAsyncOperation<SpeechRecognitionUIResult> recoOperation;

        MainViewModel viewModel;

        bool disableSpeech = true;

        StateModel currentState = new StateModel();

        // temp. remove later
        ApplicationBarIconButton micBtn;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // kick off geolocation listener
            Please2.Util.Location.StartTrackingGeolocation();

            viewModel = new MainViewModel();

            DataContext = viewModel;

            currentState.PropertyChanged += OnStateChanged;

            micBtn = (ApplicationBar.Buttons[0] as ApplicationBarIconButton);


            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();

            name.NameValue = InputScopeNameValue.Text;
            scope.Names.Add(name);

            // remove manualinput at later date
            ManualInput.InputScope = scope;
            SpeakTextBox.InputScope = scope;

            /*
            // Datetime.BuildDatetimeFromJson() test
            var test = new Newtonsoft.Json.Linq.JObject();

            var tArry = new Newtonsoft.Json.Linq.JArray();

            tArry.Add("#date_now");
            tArry.Add(1);

            test.Add("#date_add", tArry);

            var resp = Datetime.BuildDatetimeFromJson(test, null);

            Debug.WriteLine(resp["date"], resp["time"]);
            */

            
            /*
            // BuildDateTime() test
            var testPayloadString = "{\"duration\":null,\"start_date\":{\"#date_add\":[\"#date_now\", 1]},\"location\":null}";

            var testPayloadDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(testPayloadString);

            BuildDateTime(testPayloadDict);
            */

            /*
            // FindOrReplace() test
            mainContext = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassifierModel>("{\"action\":\"create\",\"model\":\"hotel_booking\",\"payload\":{\"duration\":null,\"start_date\":null,\"location\":null}}");

            Debug.WriteLine("before Cursive");
            Debug.WriteLine(SerializeData(mainContext));

            FindOrReplace("payload.location", null);

            Debug.WriteLine("after Cursive");
            Debug.WriteLine(SerializeData(mainContext));
            */

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            /*
            // shopping results test
            string shoppingResults = "[{\"title\":\"Melissa & Doug Princess Soft Toys Skimmer Dolphin\",\"price\":\"$10.14\",\"image\":\"http://ecx.images-amazon.com/images/I/517I0gXrKIL._SL160_.jpg\",\"url\":\"http://www.amazon.com/Melissa-Doug-Princess-Skimmer-Dolphin/dp/B004PBF1WW%3FSubscriptionId%3D016S53A6N2MY0NZRTAR2%26tag%3Ditemsid-20%26linkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB004PBF1WW\"},{\"title\":\"Gifts & Decor Spun Glass Dolphin Carousel Mirrored Base Figurine\",\"price\":\"$11.98\",\"image\":\"http://ecx.images-amazon.com/images/I/41REHPZ2Z4L._SL160_.jpg\",\"url\":\"http://www.amazon.com/Gifts-Decor-Carousel-Mirrored-Figurine/dp/B008YQ4Q70%3FSubscriptionId%3D016S53A6N2MY0NZRTAR2%26tag%3Ditemsid-20%26linkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB008YQ4Q70\"},{\"title\":\"Dolphin Browser v8.7\",\"price\":\"$0.00\",\"image\":\"http://ecx.images-amazon.com/images/I/61wD8OVpjBL._SL160_.png\",\"url\":\"http://www.amazon.com/MoboTap-Inc-Dolphin-Browser-v8-7/dp/B0090C0VVC%3FSubscriptionId%3D016S53A6N2MY0NZRTAR2%26tag%3Ditemsid-20%26linkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB0090C0VVC\"}]";

            App.ShoppingViewModel.ShoppingResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShoppingModel>>(shoppingResults);

            NavigationService.Navigate(new Uri("/Pages/ShoppingPage.xaml", UriKind.Relative));

            return;
            */

            // set placeholder text
            SpeakTextBox.Text = PLACEHOLDER_TEXT;

            try
            {
                if (synthesizer == null)
                {
                    synthesizer = new SpeechSynthesizer();
                }

                if (recognizer == null)
                {
                    recognizer = new SpeechRecognizerUI();
                    recognizer.Settings.ReadoutEnabled = false;
                    recognizer.Settings.ShowConfirmation = false;
                }

                //await HandleUserInput("Book a hotel in Abell");
                
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        protected void PreferencesButton_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/PreferencesPage.xaml", UriKind.Relative));
        }

        protected void RemindersButton_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/RemindersPage.xaml", UriKind.Relative));
        }

        protected void AppointmentButton_Click(object sender, EventArgs e)
        {
            var appt = new SaveAppointmentTask();

            appt.Show();
        }

        protected void SpeakTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                string query = SpeakTextBox.Text;

                SpeakTextBox.Text = String.Empty;

                HandleUserInput(query);
            }
        }

        protected void SpeakTextBox_GotFocus(object sender, EventArgs e)
        {
            if (SpeakTextBox.Text == PLACEHOLDER_TEXT)
                SpeakTextBox.Text = String.Empty;
        }

        protected void SpeakTextBox_LostFocus(object sender, EventArgs e)
        {
            if (SpeakTextBox.Text == String.Empty)
                SpeakTextBox.Text = PLACEHOLDER_TEXT;
        }

        protected void ShowDialog()
        {
            if (DialogGrid.Visibility == Visibility.Collapsed)
            {
                InputGrid.Visibility = Visibility.Collapsed;
                DialogGrid.Visibility = Visibility.Visible;
            }
        }

        protected void ShowInput()
        {
            if (InputGrid.Visibility == Visibility.Collapsed)
            {
                DialogGrid.Visibility = Visibility.Collapsed;
                InputGrid.Visibility = Visibility.Visible;
            }
        }

        // temp. remove later
        protected void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                var testInput = ManualInput.Text;

                ManualInput.Text = String.Empty;

                HandleUserInput(testInput);
            }
        }

        protected void ManualInputGotFocus(object sender, EventArgs e)
        {
            if (micBtn != null)
                micBtn.IsEnabled = false;
        }

        protected void ManualInputLostFocus(object sender, EventArgs e)
        {
            if (micBtn != null)
                micBtn.IsEnabled = true;
        }

        protected void InputMenuItem(object sender, EventArgs e)
        {
            var toggle = (sender as ApplicationBarMenuItem);

            if (ManualInput.Visibility.Equals(Visibility.Collapsed))
            {
                ManualInput.Visibility = Visibility.Visible;
                toggle.Text = "Hide Input";
                disableSpeech = true;
            }
            else
            {
                ManualInput.Visibility = Visibility.Collapsed;
                toggle.Text = "Show Input";
                disableSpeech = false;
            }
        }
        // end temp

        protected async void Microphone_Tapped(object sender, EventArgs e)
        {
            synthesizer.CancelAll();

            if (recoOperation != null && recoOperation.Status == AsyncStatus.Started)
            {
                recoOperation.Cancel();
            }

            try
            {
                recoOperation = recognizer.RecognizeWithUIAsync();

                //micBtn.IsEnabled = false;

                var recoResult = await recoOperation;

                // reset the mic button if the user cancels the recognition gui
                if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Cancelled)
                {
                    //micBtn.IsEnabled = true;
                }
                else if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    string query = recoResult.RecognitionResult.Text;

                    // replace profanity text
                    query = Regex.Replace(query, @"<profanity>(.*?)</profanity>", new MatchEvaluator(ProfanityFilter), RegexOptions.IgnoreCase);

                    // is this check needed in a succeeded state
                    if (recoResult.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.Rejected)
                    {
                        //Say("please", "I didn't quite catch that. Can you say it again?");
                    }
                    else
                    {
                        HandleUserInput(query);
                    }
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {

            }
            catch (WebException webErr)
            {
                Debug.WriteLine(webErr.ToString());
            }
            catch (Exception err)
            {
                //MicrophoneBtn.IsEnabled = true;

                const int privacyPolicyHResult = unchecked((int)0x80045509);

                if (err.HResult == privacyPolicyHResult)
                {
                    MessageBox.Show("To run this sample, you must first accept the speech privacy policy. To do so, navigate to Settings -> speech on your phone and check 'Enable Speech Recognition Service' ");
                }
                else
                {
                    Debug.WriteLine(err.ToString());
                    // textBlock.Text = "Error: " + err.Message;
                }
            } 
        }

        protected void HandleUserInput(string query)
        {
            Show("user", query);

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

        protected void Show(ResponderModel response)
        {
            string show = null;
            // parse and pass to Show
            if (response.show.simple.ContainsKey("text"))
            {
                show = (string)response.show.simple["text"];
            }

            Show("please", response.speak, show);
        }

        protected void Show(string type, string speak = "", object show = null)
        {
            try
            {
                if (show == null)
                {
                    show = speak;
                }
                else if (show.GetType() == typeof(string) && String.IsNullOrEmpty((string)show))
                {
                    show = speak;
                }

                // display response
                if (show != null)
                {
                    viewModel.AddDialog(type, show);
                    ShowDialog();
                    //ScrollTo();
                }

                // say response
                if (type.ToLower() == "please" && speak != "" && disableSpeech == false)
                {
                    synthesizer.SpeakTextAsync(speak);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("exception");
                Debug.WriteLine(err.ToString());
            }
        }

        protected async Task<Dictionary<string, object>> GetDeviceInfo()
        {
            Dictionary<string, object> deviceInfo = new Dictionary<string, object>();

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
                deviceInfo["lat"] = (string)geolocation["latitude"];
                deviceInfo["lon"] = (string)geolocation["longitude"];
            }
            else if (geolocation.ContainsKey("error"))
            {
                //ran into error acquiring geolocation
                deviceInfo["lat"] = "";
                deviceInfo["lon"] = "";

                Debug.WriteLine(geolocation["error"]);
            }

            return deviceInfo;
        }

        private async void OnStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();

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
                }
            }
        }

        protected async Task Classify(string query)
        {
            var req = new Request();

            var classifierResults = await req.DoRequestJsonAsync<ClassifierModel>(AppResources.ClassifierEndpoint + "?query=" + query);

            await Auditor(classifierResults);
        }

        protected async void DisambiguateActive(string data)
        {
            List<string> types = new List<string>();

            string  field = tempContext.field;

            string type = tempContext.type;

            types.Add(type);

            string payload = data;

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.types = types;

            var req = new Request();

            req.Method = "POST";
            req.ContentType = "application/json";

            try
            {
                Dictionary<string, object> response = await req.DoRequestJsonAsync<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/active", SerializeData(postData));
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

            var deviceInfo = await GetDeviceInfo();

            var postData = new DisambiguatorModel();

            postData.payload = payload;
            postData.type = type;
            postData.types = types;
            postData.device_info = deviceInfo;

            var req = new Request();

            req.Method = "POST";
            req.ContentType = "application/json";

            Dictionary<string, object> response = await req.DoRequestJsonAsync<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/passive", SerializeData(postData));

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

            var req = new Request();

            req.Method = "POST";
            req.ContentType = "application/json";

            Dictionary<string, object> response = await req.DoRequestJsonAsync<Dictionary<string, object>>(AppResources.PudEndpoint, SerializeData(postData));

            // hand off response to disambig response handler
            DisambiguateResponseHandler(response, field, type);
        }

        protected async void DisambiguateResponseHandler(Dictionary<string, object> response, string field, string type)
        {
            if (response != null)
            {
                response = ReplaceLocation(response);

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

                Debug.WriteLine("Disambiguate Response");
                Debug.WriteLine(SerializeData(request));

                await Auditor(request);
            }
            else
            {
                Debug.WriteLine("oops no responder response");
            }
        }

        protected async Task Auditor(ResponderModel data)
        {
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

                data["payload"] = ReplaceLocation(payload);
                data["payload"] = BuildDateTime(payload);
            }

            var req = new Request();

            req.Method = "POST";
            req.ContentType = "application/json";

            Debug.WriteLine("auditor request");
            Debug.WriteLine(SerializeData(mainContext));

            ResponderModel response = await req.DoRequestJsonAsync<ResponderModel>(AppResources.ResponderEndpoint + "audit", SerializeData(mainContext));

            string state = response.status.Replace(" ", "");

            if (state == "inprogress")
                tempContext = response;

            // create event and trigger based on status
            currentState.Response = response;
            currentState.State = state;
        } 

        protected async void Actor(ResponderModel data)
        {
            string actor = data.actor;

            var req = new Request();

            req.Method = "POST";
            req.ContentType = "application/json";

            Debug.WriteLine("Actor");
            Debug.WriteLine(SerializeData(mainContext));

            var response = await req.DoRequestJsonAsync<ActorModel>(AppResources.ResponderEndpoint + "actors/" + actor, SerializeData(mainContext));

            if (response.show.structured != null && response.show.structured.ContainsKey("template"))
            {
                Dictionary<string, object> structured = response.show.structured;
                String[] template = (structured["template"] as string).Split(':');

                // get template type and map structured data
                switch (actor.ToLower())
                {
                    case "shopping":
                    case "product":
                        Debug.WriteLine("shopping/products results");
                   
                        if (structured.ContainsKey("items"))
                        {
                            var shoppingData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShoppingModel>>(SerializeData(structured["items"]));

                            Debug.WriteLine(SerializeData(shoppingData));

                            App.ShoppingViewModel.ShoppingResults = shoppingData;

                            NavigationService.Navigate(new Uri("/Pages/ShoppingPage.xaml?template=" + structured["template"], UriKind.Relative));
                        }
                        break;

                    case "weather":
                        // set data and navigate to weather page
                        break;

                    case "event":
                        {
                            var eventData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EventModel>>(SerializeData(structured["items"]));

                            Debug.WriteLine(SerializeData(eventData));

                            App.EventsViewModel.EventResults = eventData;

                            NavigationService.Navigate(new Uri("/Pages/EventPage.xaml?template=" + structured["template"], UriKind.Relative));
                        }
                        break;

                    default:
                        // show text
                        Actor(response.show);
                        break;
                }
            }
            else
            {
                Actor(response.show);
            }
        }

        protected void Actor(ShowModel show)
        {
            if (show.simple.ContainsKey("text"))
            {
                var simple = show.simple;

                string link = null;

                if (simple.ContainsKey("link"))
                    link = (string)simple["link"];

                viewModel.AddDialog("please", simple["text"], link);
            }
        }

        #region helpers
        // if type is null, it's a find. 
        // if type has a value, it's a replace
        protected object FindOrReplace(string field, object type = null)
        {
            var fields = field.Split('.').ToList();

            // turn our context into a jobject we can work with
            var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(SerializeData(mainContext));

            var stackToVisit = new Stack<Newtonsoft.Json.Linq.JObject>();

            stackToVisit.Push(jObject);

            // keep visiting iterating until the stack of dictionaries to visit is empty
            while (stackToVisit.Count > 0)
            {
                var nextObject = stackToVisit.Pop();

                if (nextObject != null)
                {
                    foreach (var keyValuePair in nextObject)
                    {
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

        protected Dictionary<string, object> ReplaceLocation(Dictionary<string, object> payload)
        {
            if (payload.ContainsKey("location"))
            {
                if (payload["location"].GetType() == typeof(string))
                {
                    var location = (string)payload["location"];
                    if (location.Contains("current_location"))
                    {
                        payload["location"] = Location.CurrentPosition;
                    }
                }
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

        protected string SerializeData(object data)
        {
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

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

        protected void ScrollTo()
        {
            var child = VisualTreeHelper.GetChild(DialogList, 0);

            if (child is ScrollViewer)
            {
                var scrollViewer = child as ScrollViewer;

                // get last item
                var index = viewModel.DialogList.Count - 1;

                scrollViewer.ScrollToVerticalOffset(index);
                scrollViewer.UpdateLayout();
            }
        }
        #endregion

        private async void MyTest()
        {
            try
            {
                Dictionary<string, object> disambig;

                Debug.WriteLine("acquire new disambig value");

                var req = new Request();

                req.Method = "POST";
                req.ContentType = "application/json";

                string[] types = new string[] { "date" };

                var data = new Dictionary<string, object>();

                data.Add("text", "tomorrow");
                data.Add("types", types);

                var jsonSettings = new JsonSerializerSettings();

                jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data, jsonSettings);

                Debug.WriteLine(serialized);

                disambig = await req.DoRequestJsonAsync<Dictionary<string, object>>(AppResources.DisambiguatorEndpoint + "/active", serialized);


                if ((disambig.ContainsKey("date") && disambig["date"].GetType() != typeof(string)) || (disambig.ContainsKey("time") && disambig["time"].GetType() != typeof(string)))
                {
                    Newtonsoft.Json.Linq.JObject date = null;
                    Newtonsoft.Json.Linq.JObject time = null;

                    if (disambig.ContainsKey("date"))
                    {
                        date = (Newtonsoft.Json.Linq.JObject)disambig["date"];
                        /*
                        Debug.WriteLine(date.GetType());

                        foreach (var i in date)
                        {
                            var val = (Newtonsoft.Json.Linq.JArray)i.Value;
                            
                            Debug.WriteLine(i.Key + "--" + i.Value.GetType());

                            foreach (var v in val)
                            {
                                Debug.WriteLine(v);
                                Debug.WriteLine(v.GetType());
                            }
                        }
                        */
                    }

                    if (disambig.ContainsKey("time"))
                    {
                        time = (Newtonsoft.Json.Linq.JObject)disambig["time"];
                    }

                    var response = Please2.Util.Datetime.BuildDatetimeFromJson(date, time);

                    foreach (var item in response)
                    {
                        Debug.WriteLine(item.Key + "--" + item.Value);
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }
    }
}