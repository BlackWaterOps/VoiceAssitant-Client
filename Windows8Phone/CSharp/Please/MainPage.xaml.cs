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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

using Newtonsoft.Json;

using Please.Resources;
using Please.Models;

namespace Please
{
    public partial class MainPage : PhoneApplicationPage
    {
        SpeechSynthesizer _synthesizer;
        SpeechRecognizerUI _recognizer;

        IAsyncOperation<SpeechRecognitionUIResult> _recoOperation;

        Geolocator geolocator;
        Geoposition myposition;

        ApplicationBarIconButton micBtn;

        bool disableSpeech = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.PleaseViewModel;

            micBtn = (ApplicationBar.Buttons[0] as ApplicationBarIconButton);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
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

                if (e.NavigationMode == NavigationMode.Back && App.userChoice != "" && App.userChoice != null)
                {
                    var userChoice = App.userChoice;

                    App.userChoice = null;

                    addAppBarItem(String.Empty);

                    await Say("user", userChoice);

                    await makeRequest(userChoice); 
                }

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

                
                App.DatePickerViewModel.SearchTerm = "testing text";
                App.DatePickerViewModel.DefaultDate = DateTime.Now;
                addAppBarItem("datepickerpage");
                

                /* test dialog. should setup a sample viewModel but hey, this is a demo! */
                /*
                App.PleaseViewModel.PleaseList.Clear();
                App.PleaseViewModel.AddDialog("user", "testing 1");
                App.PleaseViewModel.AddDialog("please", "testing 2", "http://m.samuru.com/?python");
                App.PleaseViewModel.AddDialog("user", "testing 3");
                App.PleaseViewModel.AddDialog("please", "testing 4");
                App.PleaseViewModel.AddDialog("user", "testing 5");
                App.PleaseViewModel.AddDialog("please", "testing 6");
                */
                
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }

            base.OnNavigatedTo(e);
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

        protected void ListPicker(object sender, EventArgs e)
        {
             NavigationService.Navigate(new Uri("/ListPickerPage.xaml", UriKind.Relative));
        }

        protected void DatePicker(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DatePickerPage.xaml", UriKind.Relative));
        }
        /*
        protected async void CaptureTestText(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                disableSpeech = true;

                var testInput = TestInput.Text;

                TestInput.Text = String.Empty;

                addAppBarItem(String.Empty);

                await Say("user", testInput, testInput);

                await makeRequest(testInput);
            }
        }
        */
        protected void TestInputGotFocus(object sender, EventArgs e)
        {
            if (micBtn != null)
            {
                micBtn.IsEnabled = false;
            }
        }

        protected void TestInputLostFocus(object sender, EventArgs e)
        {
            if (micBtn != null)
            {
                micBtn.IsEnabled = true;
            }
        }

        protected async void CancelButton(object sender, EventArgs e)
        {
            try
            {
                SystemTray.ProgressIndicator.IsVisible = true;

                string requestString = await buildRequest("nevermind");;

                Debug.WriteLine(requestString);

                var req = new Please.Util.Request();

                //req.AcceptType = "json";
                req.ContentType = "application/json";
                req.Method = "POST";

                var response = await req.DoRequestJsonAsync<PleaseModel>(AppResources.Endpoint, requestString);

                Debug.WriteLine(response.ToString());
           
                SystemTray.ProgressIndicator.IsVisible = false;

                // clear out context
                App.requestContext = response.context;

                if (response.speak != null && response.speak != "REPLACE_WITH_DEVICE_TIME")
                {
                    Debug.WriteLine(response.speak);
                    await Say("please", response.speak);
                }
            }
            catch (WebException err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        protected async void PleaseButton(object sender, EventArgs e)
        {
            micBtn = (sender as ApplicationBarIconButton);

            addAppBarItem(String.Empty);

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

                    await Say("user", request);

                    // is this check needed in a succeeded state
                    if (recoResult.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.Rejected)
                    {
                        await Say("please", "I didn't quite catch that. Can you say it again?");
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
                    Debug.WriteLine("speech exception thrown");
                    Debug.WriteLine(err.ToString());
                    // textBlock.Text = "Error: " + err.Message;
                }
            }   
        }

        protected async Task makeRequest(string request)
        {
            try
            {
                SystemTray.ProgressIndicator.IsVisible = true;

                //strip punctuations & lowercase before sending to server
                //request = Regex.Replace(request, @"[^A-Za-z0-9'\s]", "").ToLower();

                string requestString = await buildRequest(request);

                Debug.WriteLine(requestString);

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

                Debug.WriteLine(response.ToString());

                SystemTray.ProgressIndicator.IsVisible = false;

                // update and hold response context
                Debug.WriteLine(response.context);
                App.requestContext = response.context;
                Debug.WriteLine(response.speak);
                Debug.WriteLine(response.show);

                if (response.show != null)
                {
                    switch ((response.show.type).ToLower())
                    {
                        case "string":
                        case "standard":
                            await Say("please", response.speak, response.show.text);
                            break;

                        case "list":
                            if (response.show.list.Count > 0)
                            {
                                App.ListPickerViewModel.SearchTerm = response.show.text;
                                App.ListPickerViewModel.LoadList(response.show.list);
                                addAppBarItem("listpickerpage");
                            }

                            await Say("please", response.speak, response.show.text);
                            break;

                        case "date":
                            App.DatePickerViewModel.SearchTerm = response.show.text;
                            App.DatePickerViewModel.DefaultDate = DateTime.Now;
                            addAppBarItem("datepickerpage");
                            await Say("please", response.speak, response.show.text);
                            break;
                    }
                }
                else if (response.speak != null && response.speak != "REPLACE_WITH_DEVICE_TIME")
                {
                    
                    if (response.trigger.action == null || ((string)response.trigger.action).ToLower() != "link")
                    {
                        Debug.WriteLine("say please response");
                        await Say("please", response.speak);
                    }
                }

                if (micBtn != null)
                {
                    micBtn.IsEnabled = true;
                }

                Debug.WriteLine("before trigger action");

                if (response.trigger.action != null && response.trigger.action != "")
                {
                    /* dynamic way to call methods
                    Type type = typeof(Please.MainPage);
                    MethodInfo info = type.GetMethod(response.trigger.action);
                    info.Invoke(this, new object[] { response.trigger.payload });
                    */

                    switch (((string)response.trigger.action).ToLower())
                    {
                        case "clear_log":
                            App.PleaseViewModel.PleaseList.Clear();
                            addAppBarItem(String.Empty);
                            break;

                        case "call":
                            DoCall(response.trigger.payload);
                            break;

                        case "sms":
                            DoSms(response.trigger.payload);
                            break;

                        case "link":

                            //var hyperlink = new HyperlinkButton();
                            var url = (string)response.trigger.payload["url"];
                            //hyperlink.NavigateUri = new Uri(url, UriKind.Absolute);
                            //hyperlink.TargetName = "_blank";
                            //hyperlink.Content = "Click for more";

                            await Say("please", response.speak, response.speak, url);
                            break;

                        case "web":
                            DoWeb(response.trigger.payload);
                            break;

                        case "email":
                            DoEmail(response.trigger.payload);
                            break;

                        case "locate":
                            if (myposition == null)
                            {
                                await GetGeolocation();
                            }

                            DoLocate(response.trigger.payload);
                            break;

                        case "directions":
                            if (myposition == null)
                            {
                                await GetGeolocation();
                            }

                            DoDirections(response.trigger.payload);
                            break;

                        case "time":
                            string time = DoTime(response.trigger.payload);
                            await Say("please", time, time);
                            break;

                        case "calendar":
                            DoCalendar(response.trigger.payload);
                            break;

                        case "images":
                            App.GalleryViewModel.SearchTerm = response.show.text;
                            App.GalleryViewModel.LoadImages(response.trigger.payload);
                            NavigationService.Navigate(new Uri("/GalleryPage.xaml", UriKind.Relative));
                            break;
                        
                        case "app_view":
                        case "view":
                            KeyValuePair<string, object> uri = response.trigger.payload.ElementAt(0);

                            await Windows.System.Launcher.LaunchUriAsync(new Uri((string)uri.Value));
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

        protected async Task Confused()
        {
            SystemTray.ProgressIndicator.IsVisible = false;
            micBtn.IsEnabled = true;
            await Say("please", "I'm sorry. I didn't understand that.");
        }

        protected async Task Say(String type, String speak = "", String show = "", String link = "")
        {
            try
            {
                show = (show == null || show == "") ? speak : show;

                // display response
                if (show != "")
                {
                    App.PleaseViewModel.AddDialog(type, show, link);
                    ScrollTo();
                }

                // say response
                if (type.ToLower() == "please" && speak != "" && disableSpeech != true)
                {
                    await _synthesizer.SpeakTextAsync(speak);
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
            App.deviceInfo = new Please.Models.Device();

            // Fri May 17 2013 11:05:06 GMT-0700 (MST)"
            // deviceInfo.time = String.Format("{0,3:ddd} {0,4:MMM} {0,2:dd} {0,4:yyyy} {0,2:hh}:{0,2:mm}:{0,2:ss} GMT{0,4:zzz}", DateTime.Now).Replace(":", "");
            App.deviceInfo.timestamp = Please.Util.Datetime.ConvertToUnixTimestamp(DateTime.Now);
            App.deviceInfo.timeoffset = DateTimeOffset.Now.Offset.Hours; // this is not good enough for world but good enough for demo

            var geolocation = await GetGeolocation();

            if (!geolocation.ContainsKey("error") && geolocation.Count > 1)
            {
                App.deviceInfo.lat = geolocation["latitude"];
                App.deviceInfo.lon = geolocation["longitude"];
            }
            else if (geolocation.ContainsKey("error"))
            {
                //ran into error acquiring geolocation
                App.deviceInfo.lat = "";
                App.deviceInfo.lon = "";

                Debug.WriteLine(geolocation["error"]);
            }

            // if context isn't set, this must be the first request.
            if (App.requestContext == null)
            {
                App.requestContext = new Please.Models.Context();
            }

            // add new device info to context
            App.requestContext.device = App.deviceInfo;

            // make request payload
            var rm = new RequestModel();

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

        #region Actions/Tasks
        protected void DoCall(Dictionary<string, object> payload)
        {
            // use PhoneCallTask
            // payload.phone
            if (payload.ContainsKey("phone")) 
            {
                var phoneCallTask = new PhoneCallTask();
                
                phoneCallTask.PhoneNumber = (string)payload["phone"];

                phoneCallTask.Show();
            }
        }

        protected void DoSms(Dictionary<string, object> payload)
        {
            // use SmsComposeTask
            // payload.phone
            // payload.message

            if (payload.ContainsKey("phone") && payload.ContainsKey("message"))
            {
                var smsComposeTask = new SmsComposeTask();

                smsComposeTask.To = (string)payload["phone"];
                smsComposeTask.Body = (string)payload["message"];

                smsComposeTask.Show();
            }
        }

        protected void DoEmail(Dictionary<string, object> payload)
        {
            // use EmailComposeTask
            // payload.subject
            // payload.message
            // payload.address
            if (payload.ContainsKey("subject") && payload.ContainsKey("message") && payload.ContainsKey("address"))
            {
                var emailComposeTask = new EmailComposeTask();

                emailComposeTask.Subject = (string)payload["subject"];
                emailComposeTask.Body = (string)payload["message"];
                emailComposeTask.To = (string)payload["address"];

                emailComposeTask.Show();
            }
        }

        protected void DoWeb(Dictionary<string, object> payload)
        {
            // use WebBrowserTask
            // payload.url
            if (payload.ContainsKey("url"))
            {
                var webBrowserTask = new WebBrowserTask();

                webBrowserTask.Uri = new Uri((string)payload["url"], UriKind.Absolute);

                webBrowserTask.Show();
            }
        }

        protected void DoCalendar(Dictionary<string, object> payload)
        {
            // use SaveAppoinmentTask
            // payload.time
            // payload.date
            // payload.duration
            // payload.location
            // payload.subject
            // payload.person
            if (payload.ContainsKey("date") && payload.ContainsKey("duration") && payload.ContainsKey("subject"))
            {
                var saveAppointmentTask = new SaveAppointmentTask();
                var location = (payload.ContainsKey("location") && payload["location"] != null) ? (string)payload["location"] : "";
                var details = (string)payload["subject"];

                if (payload.ContainsKey("person") && payload["person"] != null)
                {
                    details += " with " + (string)payload["person"];
                }

                if (location != "")
                {
                    details += " at " + location;
                }

                var time = (payload.ContainsKey("time") && payload["time"] != null) ? (string)payload["time"] : "12:00:00 PM";

                var startTime = DateTime.Parse((string)payload["date"] + " " + time);
                
                var endTime = startTime.AddMilliseconds( ((double)payload["duration"] * 3600000) );
                
                saveAppointmentTask.Subject = details;
                saveAppointmentTask.Location = location;
                saveAppointmentTask.StartTime = startTime;
                saveAppointmentTask.EndTime = endTime;
                saveAppointmentTask.Reminder = Reminder.ThirtyMinutes;
                saveAppointmentTask.AppointmentStatus = Microsoft.Phone.UserData.AppointmentStatus.Tentative;

                saveAppointmentTask.Show();
            }
        }

        protected string DoTime(Dictionary<string, object> payload)
        {
            var now = DateTime.Now;

            // say("It is now " + hours + ":" + mins + " " + ampm + " on " + day + ", " + month + " " +  date + ", " + year + ".");
            //elapseString = String.Format("{0,2:MM}/{0,2:dd}/{0,4:yyyy} {0,2:hh}:{0,2:mm}", date);

            return String.Format("It is now {0,2:hh}:{0,2:mm} {0,2:tt} on {0,2:dddd}, {0,2:MMMM} {0,2:dd}, {0,4:yyyy}", now);
        }

        protected void DoDirections(Dictionary<string, object> payload)
        {
            // use BingMapsDirectionsTask or MapsDirectionTask
            // payload.location
            if (payload.ContainsKey("location"))
            {
                /* ENDPOINT CURRENTLY DOES NOT SUPPORT THIS FEATURE */
                var mapsDirectionTask = new MapsDirectionsTask();

                var start = new LabeledMapLocation();
                var startLocation = new System.Device.Location.GeoCoordinate(myposition.Coordinate.Latitude, myposition.Coordinate.Longitude);
                start.Location = startLocation;
                
                var end = new LabeledMapLocation();
                // if payload["location"] is latlong value
                //var endLocation = new System.Device.Location.GeoCoordinate();
                //end.Location = endLocation;

                //if payload["location"] is city/state value
                end.Label = (string)payload["location"];

                mapsDirectionTask.Start = start;
                mapsDirectionTask.End = end;

                mapsDirectionTask.Show();
            }
        }

        protected void DoLocate(Dictionary<string, object> payload)
        {
            var mapsTask = new MapsTask();
            var center = new System.Device.Location.GeoCoordinate(myposition.Coordinate.Latitude, myposition.Coordinate.Longitude);
   
            if (payload.ContainsKey("coordinates"))
            {
                // payload would need to send lat/long coords
                // center = new System.Device.Location.GeoCoordinate(payload.Latitude, payload.Longitude);
            }
             
            if (payload.ContainsKey("searchterm"))
            {
               // var searchTerm = (string)payload["searchterm"];
               // mapsTask.SearchTerm = searchTerm;
            }

            mapsTask.Center = center;
            mapsTask.Show();
        }
        #endregion

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}