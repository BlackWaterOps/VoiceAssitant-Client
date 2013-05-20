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

        Please.Models.Context requestContext;
        Please.Models.Device deviceInfo;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.PleaseViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
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
                }

                SystemTray.ProgressIndicator = new ProgressIndicator();
                SystemTray.ProgressIndicator.IsIndeterminate = true;
                
                /*
                string testQuery = "call";
                Debug.WriteLine("before buildRequest");
                string testRequest = await buildRequest(testQuery);
                Debug.WriteLine("after buildRequest");
                Debug.WriteLine(testRequest);
                */

                /* test dialog. should setup a sample viewModel but hey, this is a demo!
                App.PleaseViewModel.AddDialog("user", "testing 1");
                App.PleaseViewModel.AddDialog("please", "testing 2");
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

        protected async void CancelButton(object sender, EventArgs e)
        {
            try
            {
                SystemTray.ProgressIndicator.IsVisible = true;

                string requestString = await buildRequest("nevermind");;

                Debug.WriteLine(requestString);

                var response = await Please.Util.Request.DoRequestJsonAsync<PleaseModel>(AppResources.Endpoint, "POST", requestString);

                Debug.WriteLine(response.ToString());
           
                SystemTray.ProgressIndicator.IsVisible = false;

                // clear out context
                requestContext = response.context;

                if (response.speak != null && response.speak != "REPLACE_WITH_DEVICE_TIME")
                {
                    Debug.WriteLine(response.speak);
                    await Say("please", response.speak, response.show);
                }
            }
            catch (WebException err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        protected async void PleaseButton(object sender, EventArgs e)
        {
            var micBtn = sender as ApplicationBarIconButton;
            
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
                        SystemTray.ProgressIndicator.IsVisible = true;

                        //strip punctuations & lowercase before sending to server
                        request = Regex.Replace(request, @"[^A-Za-z0-9\s]", "").ToLower();

                        //string requestString = "{\"query\": \"" + request + "\"}";
                        string requestString = await buildRequest(request);

                        Debug.WriteLine(requestString);

                        var response = await Please.Util.Request.DoRequestJsonAsync<PleaseModel>(AppResources.Endpoint, "POST", requestString);

                        Debug.WriteLine(response.ToString());

                        SystemTray.ProgressIndicator.IsVisible = false;

                        // update and hold response context
                        requestContext = response.context;

                        if (response.speak != null && response.speak != "REPLACE_WITH_DEVICE_TIME")
                        {
                            Debug.WriteLine(response.speak);
                            await Say("please", response.speak, response.show);
                        }

                        micBtn.IsEnabled = true;

                        if (response.trigger.action != null && response.trigger.action != "")
                        {
                            Debug.WriteLine(response.trigger.action);
                            Debug.WriteLine(response.trigger.payload);

                            /* dynamic way to call methods
                            Type type = typeof(Please.MainPage);
                            MethodInfo info = type.GetMethod(response.trigger.action);
                            info.Invoke(this, new object[] { response.trigger.payload });
                            */

                            switch (((string)response.trigger.action).ToLower())
                            {
                                case "call":
                                    DoCall(response.trigger.payload);
                                    break;

                                case "sms":
                                    DoSms(response.trigger.payload);
                                    break;

                                case "link":
                                    //DoLink(response.trigger.payload);
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
                                    App.GalleryViewModel.SearchTerm = response.show;
                                    App.GalleryViewModel.LoadImages(response.trigger.payload);
                                    NavigationService.Navigate(new Uri("/GalleryPage.xaml", UriKind.Relative));
                                    break;
                            }
                        }
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
                micBtn.IsEnabled = true;

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

        protected async Task Say(String type, String speak, String show = "")
        {
            show = (show == null || show == "") ? speak : show;

            // display response
            App.PleaseViewModel.AddDialog(type, show);

            ScrollTo();

            if (type.ToLower() == "please")
            {
                await _synthesizer.SpeakTextAsync(speak);
            }
        }

        protected async Task<String> buildRequest(string query)
        {
            deviceInfo = new Please.Models.Device();

            // Fri May 17 2013 11:05:06 GMT-0700 (MST)"
            // deviceInfo.time = String.Format("{0,3:ddd} {0,4:MMM} {0,2:dd} {0,4:yyyy} {0,2:hh}:{0,2:mm}:{0,2:ss} GMT{0,4:zzz}", DateTime.Now).Replace(":", "");
            deviceInfo.timestamp = Please.Util.Datetime.ConvertToUnixTimestamp(DateTime.Now);
            deviceInfo.timeoffset = DateTimeOffset.Now.Offset.Hours; // this is not good enough for world but good enough for demo

            var geolocation = await GetGeolocation();

            if (!geolocation.ContainsKey("error") && geolocation.Count > 1)
            {
                deviceInfo.lat = geolocation["latitude"];
                deviceInfo.lon = geolocation["longitude"];
            }
            else if (geolocation.ContainsKey("error"))
            {
                //ran into error acquiring geolocation
                deviceInfo.lat = "";
                deviceInfo.lon = "";

                Debug.WriteLine(geolocation["error"]);
            }

            // if context isn't set, this must be the first request.
            if (requestContext == null)
            {
                requestContext = new Please.Models.Context();
            }

            // add new device info to context
            requestContext.device = deviceInfo;

            // create request string
            string requestString;

            requestString = "query=" + Uri.EscapeDataString(query);
            requestString += "&context=" + Uri.EscapeDataString(Newtonsoft.Json.JsonConvert.SerializeObject(requestContext));

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