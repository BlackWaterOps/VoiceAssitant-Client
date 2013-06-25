using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;
using Windows.System; // used for launcher

using Newtonsoft.Json;

using Please.Resources;
using Please.Models;
using Please.Util;

namespace Please
{
    //TODO: public partial class MainPage : PleasePageBase
    public partial class MainPage : PhoneApplicationPage
    {
        SpeechSynthesizer _synthesizer;
        SpeechRecognizerUI _recognizer;

        IAsyncOperation<SpeechRecognitionUIResult> _recoOperation;
        
        ApplicationBarIconButton micBtn;

        //Please.Util.Location _location;

        // move to Util.Location
        Geolocator geolocator;
        Geoposition myposition;

        // move to Util.Tests
        bool disableSpeech = false;

        bool isDebugging = false;

        bool testAdded = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.PleaseViewModel;

            micBtn = (ApplicationBar.Buttons[0] as ApplicationBarIconButton);

            //_location = new Please.Util.Location();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           
            try
            {
                if (_synthesizer == null)
                {
                    _synthesizer = new SpeechSynthesizer();
                }

                if (_recognizer == null)
                {
                    _recognizer = new SpeechRecognizerUI();
                    _recognizer.Settings.ReadoutEnabled = false;
                    _recognizer.Settings.ShowConfirmation = false;
                }

                SystemTray.ProgressIndicator = new ProgressIndicator();
                SystemTray.ProgressIndicator.IsIndeterminate = true;

                if (e.NavigationMode == NavigationMode.Back)
                {
                    if (App.userChoice != "" && App.userChoice != null)
                    {
                        var userChoice = App.userChoice;

                        App.userChoice = null;

                        addAppBarItem(String.Empty);

                        Say("user", userChoice);

                        await makeRequest(userChoice);
                    }
                }

                // simulate payload
                //var payload = new Dictionary<string, object>();

                //var d = new DateTime();

                //payload.Add("datetime", "2013-06-24T19:40:00");
                
                // set new alarm
                //Please.Util.Actions.DoAlarm(payload);

                /*
                List<string> testList = new List<string>();

                testList.Add("value 1");
                testList.Add("value 2");
                testList.Add("value 3");
                testList.Add("value 4");
                testList.Add("value 5");
                testList.Add("value 6");

                App.ListPickerViewModel.SearchTerm = "testing text";
                App.ListPickerViewModel.LoadList(testList);                 
                addAppBarItem("listpickerpage");
                */
                
                /*
                App.DatePickerViewModel.SearchTerm = "testing text";
                App.DatePickerViewModel.DefaultDate = DateTime.Now;
                addAppBarItem("datepickerpage");
                */

                /* test output response */
                if (testAdded == false && isDebugging == true)
                {
                    bool added = App.PleaseViewModel.addDialogTemplate();

                    if (added)
                    {
                        testAdded = true;
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);


            Debug.WriteLine("navigating away from page");

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

        # region event handlers
        protected void ListPicker(object sender, EventArgs e)
        {
             NavigationService.Navigate(new Uri("/ListPickerPage.xaml", UriKind.Relative));
        }

        protected void DatePicker(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DatePickerPage.xaml", UriKind.Relative));
        }
        
        protected async void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                disableSpeech = true;

                var testInput = TestInput.Text;

                TestInput.Text = String.Empty;

                addAppBarItem(String.Empty);

                Say("user", testInput, testInput);

                await makeRequest(testInput);
            }
        }
        
        protected void TestInputGotFocus(object sender, EventArgs e)
        {
            if (micBtn != null)
                micBtn.IsEnabled = false;
        }

        protected void TestInputLostFocus(object sender, EventArgs e)
        {
            if (micBtn != null)
                micBtn.IsEnabled = true;
        }

        protected void InputMenuItem(object sender, EventArgs e)
        {
            var toggle = (sender as ApplicationBarMenuItem);

            if (TestInput.Visibility.Equals(Visibility.Collapsed))
            {
                TestInput.Visibility = Visibility.Visible;
                toggle.Text = "Hide Input";
            }
            else
            {
                TestInput.Visibility = Visibility.Collapsed;
                toggle.Text = "Show Input";
            }
        }

        protected async void OnCancel(object sender, EventArgs e)
        {
            try
            {
                SystemTray.ProgressIndicator.IsVisible = true;

                string requestString = await buildRequest("nevermind");;

                var req = new Please.Util.Request();

                //req.AcceptType = "json";
                req.ContentType = "application/json";
                req.Method = "POST";

                var response = await req.DoRequestJsonAsync<PleaseModel>(AppResources.Endpoint, requestString);
           
                SystemTray.ProgressIndicator.IsVisible = false;

                // clear out context
                App.requestContext = response.context;

                if (response.speak != null && response.speak != "REPLACE_WITH_DEVICE_TIME")
                {
                    Debug.WriteLine(response.speak);
                    Say("please", response.speak);
                }
            }
            catch (WebException err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        protected void OnNotifications(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NotificationsPage.xaml", UriKind.Relative));
        }

        protected async void OnSpeak(object sender, EventArgs e)
        {
            micBtn = (sender as ApplicationBarIconButton);

            addAppBarItem(String.Empty);

            _synthesizer.CancelAll();
            
            // Cancel the outstanding recognition operation, if one exists 
            if (_recoOperation != null && _recoOperation.Status == AsyncStatus.Started)
            {
                _recoOperation.Cancel();
            }

            try
            {
                _recoOperation = _recognizer.RecognizeWithUIAsync();

                micBtn.IsEnabled = false;

                var recoResult = await _recoOperation;

                // reset the mic button if the user cancels the recognition gui
                if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Cancelled)
                {
                    micBtn.IsEnabled = true;
                }
                else if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    string request = recoResult.RecognitionResult.Text;

                    // replace profanity text
                    request = Regex.Replace(request, @"<profanity>(.*?)</profanity>", new MatchEvaluator(ProfanityFilter), RegexOptions.IgnoreCase);

                    Say("user", request);

                    // is this check needed in a succeeded state
                    if (recoResult.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.Rejected)
                    {
                        Say("please", "I didn't quite catch that. Can you say it again?");
                    }
                    else
                    {
                        Debug.WriteLine("make request");
                        await makeRequest(request);
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
                MicrophoneBtn.IsEnabled = true;

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
        # endregion

        protected async Task makeRequest(string request)
        {
            try
            {
                SystemTray.ProgressIndicator.IsVisible = true;

                //strip punctuations & lowercase before sending to server
                //request = Regex.Replace(request, @"[^A-Za-z0-9'\s]", "").ToLower();

                string requestString = await buildRequest(request);

                var req = new Please.Util.Request();
                //req.AcceptType = "json";
                req.ContentType = "application/json";
                req.Method = "POST";

                PleaseModel response = new PleaseModel();

                try
                {
                    response = await req.DoRequestJsonAsync<PleaseModel>(AppResources.Endpoint, requestString);
                }
                catch
                {
                    Confused();
                    return;
                }

                SystemTray.ProgressIndicator.IsVisible = false;

                // update and hold response context
                App.requestContext = response.context;

                if (micBtn != null)
                {
                    micBtn.IsEnabled = true;
                }

                if (response.show != null)
                {
                    var showType = (response.show["type"] as string);
                    var showText = "";

                    if (response.show.ContainsKey("text"))
                    {
                        showText = (response.show["text"] as string);
                    }

                    switch (showType.ToLower())
                    {
                        case "string":
                        case "standard":
                            if (!String.IsNullOrEmpty(showText))
                            {
                                Say("please", response.speak, showText);
                            }
                            break;

                        case "list":
                            Debug.WriteLine("show list");
                            if (response.show.ContainsKey("list"))
                            {
                                var showList = (Newtonsoft.Json.Linq.JArray)response.show["list"];
                             
                                if (showList.Count > 0)
                                {
                                    App.ListPickerViewModel.SearchTerm = showText;
                                    App.ListPickerViewModel.LoadList(showList);
                                    addAppBarItem("listpickerpage");
                                }

                                Say("please", response.speak, showText);
                            }
                            break;

                        case "date":
                            App.DatePickerViewModel.SearchTerm = showText;
                            App.DatePickerViewModel.DefaultDate = DateTime.Now;
                            addAppBarItem("datepickerpage");
                            Say("please", response.speak, showText);
                            break;

                        case "preformatted":
                            var showTemplate = (response.show["template"] as string);
                            
                            // re-serialize object so we can deserialize it against the appropriate template
                            string output = Newtonsoft.Json.JsonConvert.SerializeObject(response.show["output"]);
                            
                            // store output for testing
                            App.PleaseViewModel.AddOutput(output);

                            switch (showTemplate.ToLower())
                            {
                                case "shopping":

                                    var jsonSettings = new JsonSerializerSettings();

                                    jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                                    jsonSettings.NullValueHandling = NullValueHandling.Include;

                                    var showOutput = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShoppingModel>>(output, jsonSettings);

                                    //Debug.WriteLine(response.show["output"]);
                                    App.PleaseViewModel.AddDialog("please", showOutput, "ShoppingDataTemplate");
                                    //Debug.WriteLine(showOutput);
                                    //Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(showOutput));
                                    break;
                            }

                            break;
                    }
                }
                else if (response.speak != null && response.speak != "REPLACE_WITH_DEVICE_TIME")
                {
                    Say("please", response.speak);
                }

                if (response.trigger.action != null && response.trigger.action != "")
                {
                    /* dynamic way to call methods
                    Type type = typeof(Please.Util.Actions);
                    MethodInfo actions = type.GetMethod(response.trigger.action);
                    actions.Invoke(this, new object[] { response.trigger.payload });
                    */
                    Dictionary<string, object> payload = response.trigger.payload;

                    switch (((string)response.trigger.action).ToLower())
                    {
                        case "clear_log":
                            App.PleaseViewModel.PleaseList.Clear();
                            addAppBarItem(String.Empty);
                            break;

                        case "call":
                            Please.Util.Actions.DoCall(payload);
                            break;

                        case "sms":
                            Please.Util.Actions.DoSms(payload);
                            break;
                                    
                        case "link":
                            var list = App.PleaseViewModel.PleaseList;

                            var item = list.ElementAt<DialogModel>(list.Count - 1);

                            if (item.sender.Equals("please") && payload.ContainsKey("url"))
                            {
                                var url = (string)payload["url"];
                            
                                item.link = url;
                            }
                            break;

                        case "web":
                            Please.Util.Actions.DoWeb(payload);
                            break;

                        case "email":
                            Please.Util.Actions.DoEmail(payload);
                            break;

                        case "locate":
                            if (myposition == null)
                            {
                                await GetGeolocation();
                            }

                            Please.Util.Actions.DoLocate(payload, myposition);
                            break;

                        case "directions":
                            if (myposition == null)
                            {
                                await GetGeolocation();
                            }

                            Please.Util.Actions.DoDirections(payload, myposition);
                            break;

                        case "time":
                            //string time = Please.Util.Actions.DoTime(payload);
                            //Say("please", time, time);
                            break;

                        case "calendar":
                            Please.Util.Actions.DoCalendar(payload);
                            break;

                        case "images":
                            App.GalleryViewModel.SearchTerm = (response.show["text"] as string);
                            App.GalleryViewModel.LoadImages(payload);
                            NavigationService.Navigate(new Uri("/GalleryPage.xaml", UriKind.Relative));
                            break;
                        
                        case "app_view":
                        case "view":
                            await Please.Util.Actions.DoIntent(payload);
                            break;

                        case "alarm":
                            Please.Util.Actions.DoAlarm(payload);
                            break;

                        case "reminder":
                            Please.Util.Actions.DoReminder(payload);
                            break;
                    }
                }
            }
            catch 
            {
                Confused();                
            }
        }

        protected void addAppBarItem(string page)
        {
            var appBar = ApplicationBar;

            // reusable appbar btn
            ApplicationBarIconButton appBarBtn;

            if (appBar.Buttons.Count > 1)
            {
                appBar.Buttons.RemoveAt(1);
            }

            switch (page.ToLower())
            {
                case "listpickerpage":
                    // browser button
                    appBarBtn = Application.Current.Resources["ListPickerBtn"] as ApplicationBarIconButton;
                    appBar.Buttons.Insert(1, appBarBtn);
                  break;

                case "datepickerpage":
                    // settings button
                    appBarBtn = Application.Current.Resources["DatePickerBtn"] as ApplicationBarIconButton;
                    appBar.Buttons.Insert(1, appBarBtn);
                    break;
            }
        }

        protected void ScrollTo()
        {
            var child = VisualTreeHelper.GetChild(PleaseDialogList, 0);

            if (child is ScrollViewer)
            {
                var scrollViewer = child as ScrollViewer;

                // get last item
                var index = App.PleaseViewModel.PleaseList.Count - 1;

                scrollViewer.ScrollToVerticalOffset(index);
                scrollViewer.UpdateLayout();

                //if (index == App.PleaseViewModel.StoryList.Count)
                  //  PleaseDialogList.ScrollIntoView(_currentStory);
            }
        }

        protected void Confused()
        {
            SystemTray.ProgressIndicator.IsVisible = false;
            micBtn.IsEnabled = true;
            Say("please", "I'm sorry. I didn't understand that.");
        }

        protected void Say(String type, String speak = "", String show = "")
        {
            Debug.WriteLine(type);
            Debug.WriteLine(speak);

            try
            {
                show = (show == null || show == "") ? speak : show;

                // display response
                if (show != "")
                {
                    App.PleaseViewModel.AddDialog(type, show);
                    ScrollTo();
                }

                // say response
                if (type.ToLower() == "please" && speak != "" && disableSpeech == false)
                {
                    _synthesizer.SpeakTextAsync(speak);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("exception");
                Debug.WriteLine(err.ToString());
            }
        }

        protected async Task<String> buildRequest(string query)
        {
            // if context isn't set, this must be the first request.
            if (App.requestContext == null)
            {
                Debug.WriteLine("create new context");
                App.requestContext = new Dictionary<string, object>();
            }

            Newtonsoft.Json.Linq.JObject deviceInfo;

            // get current device or create a new one
            if (App.requestContext.ContainsKey("device"))
            {
                deviceInfo = (App.requestContext["device"] as Newtonsoft.Json.Linq.JObject);
            }
            else
            {
                deviceInfo = new Newtonsoft.Json.Linq.JObject();
            }

            deviceInfo["type"] = "windows8-client";
            
            // Fri May 17 2013 11:05:06 GMT-0700 (MST)"
            // deviceInfo.time = String.Format("{0,3:ddd} {0,4:MMM} {0,2:dd} {0,4:yyyy} {0,2:hh}:{0,2:mm}:{0,2:ss} GMT{0,4:zzz}", DateTime.Now).Replace(":", "");
            deviceInfo["timestamp"] = Please.Util.Datetime.ConvertToUnixTimestamp(DateTime.Now);
            deviceInfo["timeoffset"] = DateTimeOffset.Now.Offset.Hours; // this is not good enough for world but good enough for demo
          
            var geolocation = await GetGeolocation();

            if (!geolocation.ContainsKey("error") && geolocation.Count > 1)
            {
                deviceInfo["lat"] = geolocation["latitude"];
                deviceInfo["lon"] = geolocation["longitude"];
            }
            else if (geolocation.ContainsKey("error"))
            {
                //ran into error acquiring geolocation
                deviceInfo["lat"] = "";
                deviceInfo["lon"] = "";

                Debug.WriteLine(geolocation["error"]);
            }

            Debug.WriteLine("before setting device ");

            // set update context device info
            if (App.requestContext.ContainsKey("device"))
            {
                App.requestContext["device"] = deviceInfo;
            }
            else
            {
                App.requestContext.Add("device", deviceInfo);
            }
            Debug.WriteLine("after setting device ");
            // make request payload
            RequestModel rm = new RequestModel();

            rm.query = query;
            rm.context = App.requestContext;
            
            var jsonSettings = new JsonSerializerSettings();

            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            string requestString = Newtonsoft.Json.JsonConvert.SerializeObject(rm, jsonSettings);

            return requestString;
        }
        
        protected async Task<Dictionary<string, string>> GetGeolocation()
        {
            var response = new Dictionary<string, string>();

            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            geolocator.ReportInterval = 600000; // 10 min in milliseconds
            
            try
            {
                Debug.WriteLine(geolocator);

                myposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                );
                
                response.Add("latitude", myposition.Coordinate.Latitude.ToString("0.00"));
                response.Add("longitude", myposition.Coordinate.Longitude.ToString("0.00"));
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);

                if ((uint)err.HResult == 0x80004004)
                {
                    // location has been diabled in phone settings. display appropriate message
                    response.Add("error", "location is disabled in phone settings");
                }
                else
                {
                    // unforeseen error
                    response.Add("error", err.ToString());
                }
            }

            return response;
        }
    }
}