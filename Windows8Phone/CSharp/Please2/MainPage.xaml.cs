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

using LinqToVisualTree;

using Please2.Models;
using Please2.Resources;
using Please2.Util;
using Please2.ViewModels;

using Newtonsoft.Json;

// TODO: rename mainContext to requestState
// TODO: check for location permission and alert user with message
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

        bool disableSpeech = false;

        StateModel currentState = new StateModel();

        // temp. remove later
        ApplicationBarIconButton micBtn;

        int counter = 0;

        List<ShoppingModel> shoppingTest;

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
            // BuildDateTime()/ReplaceLocation() test
            var testPayloadString = "{\"duration\":null,\"start_date\":{\"#date_add\":[\"#date_now\", 1]},\"location\":\"#current_location\"}";

            var testPayloadDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(testPayloadString);

            //BuildDateTime(testPayloadDict);
            ReplaceLocation(testPayloadDict);
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

            //var myTest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>("{\"action\":\"create\",\"model\":\"hotel_booking\",\"payload\":{\"duration\":null,\"start_date\":null,\"location\":null}}");

            // Find("payload.location.city");

            // PREPEND_TO TEST
            //var test = "{\"action\": \"create\",\"model\": \"email\",\"payload\": {\"message\": \"test\",\"contact\": {\"candidates\": [\"brandon\",\"this\",\"is\",\"a\"],\"prepend_to\": \"message\"}}}";
            //mainContext = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassifierModel>(test);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // shopping results test
            /*
            string shoppingResults = "{\"structured\":{\"items\":[{\"title\":\"Melissa & Doug Princess Soft Toys Skimmer Dolphin\",\"price\":\"$10.14\",\"image\":\"http://ecx.images-amazon.com/images/I/517I0gXrKIL._SL160_.jpg\",\"url\":\"http://www.amazon.com/Melissa-Doug-Princess-Skimmer-Dolphin/dp/B004PBF1WW%3FSubscriptionId%3D016S53A6N2MY0NZRTAR2%26tag%3Ditemsid-20%26linkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB004PBF1WW\"},{\"title\":\"Gifts & Decor Spun Glass Dolphin Carousel Mirrored Base Figurine\",\"price\":\"$11.98\",\"image\":\"http://ecx.images-amazon.com/images/I/41REHPZ2Z4L._SL160_.jpg\",\"url\":\"http://www.amazon.com/Gifts-Decor-Carousel-Mirrored-Figurine/dp/B008YQ4Q70%3FSubscriptionId%3D016S53A6N2MY0NZRTAR2%26tag%3Ditemsid-20%26linkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB008YQ4Q70\"},{\"title\":\"Dolphin Browser v8.7\",\"price\":\"$0.00\",\"image\":\"http://ecx.images-amazon.com/images/I/61wD8OVpjBL._SL160_.png\",\"url\":\"http://www.amazon.com/MoboTap-Inc-Dolphin-Browser-v8-7/dp/B0090C0VVC%3FSubscriptionId%3D016S53A6N2MY0NZRTAR2%26tag%3Ditemsid-20%26linkCode%3Dxm2%26camp%3D2025%26creative%3D165953%26creativeASIN%3DB0090C0VVC\"}]}}";

            var test = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(shoppingResults);

            var structure = (Newtonsoft.Json.Linq.JObject)test["structured"];

            var items = structure["items"];

            var l = items.ToObject<List<ShoppingModel>>();

            Debug.WriteLine(l.First().title);
            */

            /*
            App.ShoppingViewModel.ShoppingResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShoppingModel>>(shoppingResults);

            NavigationService.Navigate(new Uri("/Pages/ShoppingPage.xaml", UriKind.Relative));

            return;
            */

            // set placeholder text
            SpeakTextBox.Text = PLACEHOLDER_TEXT;

            try
            {
                SystemTray.ProgressIndicator = new ProgressIndicator();

                SystemTray.ProgressIndicator.IsIndeterminate = true;

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
                Debug.WriteLine(err.Message);
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            ShowInput();
            mainContext = null;
            tempContext = null;
            counter = 0;

            ManualInput.Visibility = Visibility.Collapsed;
            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = "Show Input";
            disableSpeech = false;

            currentState = new StateModel();
            currentState.PropertyChanged += OnStateChanged;

            viewModel.DialogList.Clear();
        }

        protected void PairButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/PeerFinder.xaml", UriKind.Relative));
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
                // this.Focus(); // hide keyboard

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
                ApplicationBar.IsVisible = true;
            }
        }

        protected void ShowInput()
        {
            if (InputGrid.Visibility == Visibility.Collapsed)
            {
                ApplicationBar.IsVisible = false;
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

                disableSpeech = true;

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
                    Debug.WriteLine(err.Message);
                    return;
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
            var simple = response.show.simple;

            if (simple.ContainsKey("text") && simple["text"] != null)
            {
                show = (string)simple["text"];
            }

            Show("please", response.speak, show);
        }

        // most of the time this will be called from the Actor method
        protected void Show(ShowModel showModel, string speak = "")
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

        protected async void Show(string type, string speak = "", object show = null, string link = null)
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
                    viewModel.AddDialog(type, show, link);
                    ShowDialog();
                    ScrollTo();
                }

                // say response
                if (type.ToLower() == "please" && speak != "" && disableSpeech == false)
                {
                    await synthesizer.SpeakTextAsync(speak);
                }

                // reset counter
                counter = 0;
            }
            catch (Exception err)
            {
                Debug.WriteLine("exception");
                Debug.WriteLine(err.ToString());
            }
        }

        protected void MessageError(string message)
        {
            MessageBox.Show(message, "Server Error", MessageBoxButton.OK);
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

                    case "exception":
                        Debug.WriteLine("error status detected");
                        MessageError((string)currentState.Response);
                        break;
                }
            }
        }

        protected async Task Classify(string query)
        {
            try
            {           
                var classifierResults = await RequestHelper<ClassifierModel>((AppResources.ClassifierEndpoint + "?query=" + query), "GET");

                if (classifierResults.error != null && classifierResults.error.code > 200)
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

            string  field = tempContext.field;

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
                    if (er.code > 200)
                    {
                        currentState.Response = er.message;
                        currentState.State = "exception";
                    }
                }
                else
                {
                    response = await DoClientOperations(response);

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

                data["payload"] = await DoClientOperations(payload);
            }

            Debug.WriteLine("auditor request");
            Debug.WriteLine(SerializeData(mainContext));

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

                    Debug.WriteLine("Actor");
                    Debug.WriteLine(SerializeData(response));

                    if (response.error != null)
                    {
                        currentState.Response = response.error.msg;
                        currentState.State = "exception";
                    }
                    else
                    {
                        Show(response.show, response.speak);
                    }
                }
            }
            else
            {
                Show(data.show, data.speak);
            }

            return;

            /*
            if (response.show.structured != null && response.show.structured.ContainsKey("template"))
            {
                Dictionary<string, object> structured = response.show.structured;
                String[] template = (structured["template"] as string).Split(':');

                // get template type and map structured data
                switch (actor.ToLower())
                {
                    case "shopping":
                    case "product":
                        try
                        {
                            if (structured.ContainsKey("items"))
                            {
                                App.ShoppingViewModel.SetShoppingResults(structured["items"]);

                                NavigationService.Navigate(new Uri("/Pages/ShoppingPage.xaml?template=" + structured["template"], UriKind.Relative));
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                        }
                        break;

                    case "event":
                        if (structured.ContainsKey("items"))
                        {
                            App.EventsViewModel.SetEventResults(structured["items"]);

                            NavigationService.Navigate(new Uri("/Pages/EventPage.xaml?template=" + structured["template"], UriKind.Relative));
                        }
                        break;

                    default:
                        // show text
                        //Show(response.show, response.speak);
                        break;
                }
            }
            else
            {
                //Show(response.show, response.speak);
            }
             */
        }

        #region helpers
        protected async Task<T> RequestHelper<T>(string endpoint, string method, object data = null)
        {
            var req = new Request();

            req.Method = method;

            SystemTray.ProgressIndicator.IsVisible = true;

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

            SystemTray.ProgressIndicator.IsVisible = false;

            return response;
        }

        protected async Task<Dictionary<string, object>> DoClientOperations(Dictionary<string, object> response)
        {
            response = await ReplaceLocation(response);
            response = BuildDateTime(response);
            response = PrependTo(response);

            return response;
        }

        protected Dictionary<string, object> PrependTo(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("unused_tokens"))
            {
                return data;
            }

            var prepend = (string)((Newtonsoft.Json.Linq.JArray)data["unused_tokens"]).Aggregate((i, j) => i + " " + j);

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

        protected async Task<Dictionary<string, object>> ReplaceLocation(Dictionary<string, object> payload)
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

        protected void ScrollTo()
        {
            // get last item
            var index = viewModel.DialogList.Count - 1;

            var enumerable = DialogList.Descendants<ScrollViewer>().Cast<ScrollViewer>();

            if (enumerable.Count() > 0)
            {
                var scrollViewer = enumerable.Single();

                scrollViewer.ScrollToVerticalOffset(index);
                scrollViewer.UpdateLayout();
            }
        }
        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (sender as MenuItem).DataContext as DialogModel;

            Debug.WriteLine(dialog.message);

            Clipboard.SetText((string)dialog.message);
        }
    }
}