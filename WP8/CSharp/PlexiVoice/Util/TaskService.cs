using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.ViewModels;

using PlexiSDK;
using PlexiSDK.Models;

//TODO: major refactor
namespace PlexiVoice.Util
{
    class TaskService : ITaskService
    {
        private Dictionary<string, object> payload;

        private INavigationService navigationService;

        private IPlexiService plexiService;

        private ISpeechService speechService;

        private List<string> unused_tokens = new List<string>();

        private ClassifierModel mainContext;
        public ClassifierModel MainContext 
        { 
            get { return mainContext; } 
            set 
            { 
                mainContext = value;
                payload = value.payload;
            } 
        }

        public TaskService()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            this.speechService = ViewModelLocator.GetServiceInstance<ISpeechService>();
        }

        #region Email
        public void ComposeEmail(Dictionary<string, object> payload)
        {
            string errorMessage;

            if (payload.ContainsKey("contact"))
            {
                JObject contact = (JObject)payload["contact"];

                JToken emails;

                if (contact.TryGetValue("emails", out emails))
                {
                    JObject first = (JObject)emails.FirstOrDefault();

                    foreach (JObject email in emails)
                    {
                        if ((bool)email["default"] == true)
                        {
                            first = email;
                        }
                    }

                    //string successMessage = String.Format("sending an email to {0}", (string)contact["name"]);

                    //await this.speechService.Speak(successMessage);
                    
                    string message = (string)payload["message"];

                    string address = (string)first.GetValue("value");

                    DoEmailTask(address, message);
                    return;
                }

                // no email found message
                errorMessage = String.Format("I could not find an email address for {0}", (string)contact["name"]);
            }

            // no contact found message
            errorMessage = String.Format("No contact could be found");

            if (errorMessage != null)
            {
                //await this.speechService.Speak(errorMessage);
                /*
                PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (!frame.CurrentSource.Equals(ViewModelLocator.MainPageUri))
                {
                    this.navigationService.NavigateTo(ViewModelLocator.MainPageUri);
                }
                 */
            }
        }

        public void ComposeEmail(Contact contact)
        {
            DoEmailTask(contact, this.unused_tokens);
        }

        public void ComposeEmail()
        {
            var candidates = GetCandidates();

            if (candidates.Count > 0)
            {
                GetContacts(candidates, (contactList, unused_tokens) =>
                    {
                        if (contactList.Count == 1)
                        {
                            var contact = contactList.First();

                            DoEmailTask(contact, unused_tokens);
                        }
                        else
                        {
                            this.unused_tokens = unused_tokens;

                            ShowContactList(contactList);
                        }
                    }
                );
            }
        }
        #endregion

        #region Sms
        public void ComposeSms(Dictionary<string, object> payload)
        {
            string errorMessage;

            if (payload.ContainsKey("contact"))
            {
                JObject contact = (JObject)payload["contact"];

                JToken phoneNumbers;

                if (contact.TryGetValue("phone_numbers", out phoneNumbers))
                {
                    JArray numbers = (JArray)phoneNumbers;

                    if (numbers.Count > 0)
                    {
                        JObject primary = (JObject)numbers.FirstOrDefault();

                        foreach (JObject number in numbers)
                        {
                            if ((bool)number["default"] == true)
                            {
                                primary = number;
                            }
                        }

                        //string successMessage = String.Format("sending a text message to {0}", (string)contact["name"]);

                        //await this.speechService.Speak(successMessage);

                        string phoneNumber = (string)primary.GetValue("value");

                        string message = (string)payload["message"];

                        DoSmsTask(phoneNumber, message);
                        return;
                    }
                }

                // no phone numbers message
                errorMessage = String.Format("I could not find a phone number for {0}", (string)contact["name"]);
            }
            else
            {
                // no contact found message
                errorMessage = String.Format("No contact could be found");
            }

            if (errorMessage != null)
            {
                //await this.speechService.Speak(errorMessage);
                /*
                PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (!frame.CurrentSource.Equals(ViewModelLocator.MainPageUri))
                {
                    this.navigationService.NavigateTo(ViewModelLocator.MainPageUri);
                }
                */
            }
            
            return;
        }

        public void ComposeSms(Contact contact)
        {
            DoSmsTask(contact, this.unused_tokens);
        }

        public void ComposeSms()
        {
            Debug.WriteLine("compose sms");

            var candidates = GetCandidates();

            if (candidates.Count > 0)
            {
                GetContacts(candidates, (contactList, unused_tokens) =>
                {
                    if (contactList.Count > 0)
                    {
                        if (contactList.Count == 1)
                        {
                            var contact = contactList.First();

                            DoSmsTask(contact, unused_tokens);
                        }
                        else
                        {
                            this.unused_tokens = unused_tokens;

                            ShowContactList(contactList);
                        }
                    }
                    else
                    {
                        NoContactFound();
                    }
                });
            }
        }
        #endregion

        #region Call
        public void PhoneCall(Dictionary<string, object> payload)
        {
            string errorMessage;

            if (payload.ContainsKey("contact"))
            {
                JObject contact = (JObject)payload["contact"];

                JToken phoneNumbers;

                if (contact.TryGetValue("phone_numbers", out phoneNumbers))
                {
                    JArray numbers = (JArray)phoneNumbers;

                    if (numbers.Count > 0)
                    {
                        JObject primary = (JObject)numbers.First();

                        //string recipient = (contact["name"] != null) ? (string)contact["name"] : (string)primary.GetValue("value");
                       
                        //string successMessage = String.Format("calling {0}", recipient);

                        //await this.speechService.Speak(successMessage);

                        string phoneNumber = (string)primary.GetValue("value");

                        string displayName = (string)contact["name"];

                        DoPhoneCallTask(displayName, phoneNumber);
                        return;
                    }
                }

                // no phone numbers message
                errorMessage = String.Format("I could not find a phone number for {0}", (string)contact["name"]);
            }
            else
            {
                // no contact found message
                errorMessage = String.Format("No contact could be found");
            }

            if (errorMessage != null)
            {
                //await this.speechService.Speak(errorMessage);
                /*
                PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (!frame.CurrentSource.Equals(ViewModelLocator.MainPageUri))
                {
                    this.navigationService.NavigateTo(ViewModelLocator.MainPageUri);
                }
                */
            }
            
        }

        public void PhoneCall(Contact contact)
        {
            DoPhoneCallTask(contact);
        }

        public void PhoneCall()
        {
            var candidates = GetCandidates();

            if (candidates.Count > 0)
            {
                GetContacts(candidates, (contactList, unused_tokens) =>
                    {
                        if (contactList.Count > 0)
                        {
                            if (contactList.Count == 1)
                            {
                                var contact = contactList.First();

                                DoPhoneCallTask(contact);
                            }
                            else
                            {
                                this.unused_tokens = unused_tokens;

                                ShowContactList(contactList);
                            }
                        }
                        else
                        {
                            // dialog service error message
                            NoContactFound();
                        }
                    }
                );
            }
        }
        #endregion

        public void SetAppointment(Dictionary<string, object> payload)
        {
            string location = String.Empty;
            string startTime = String.Empty;
            string endTime = String.Empty;

            if (payload.ContainsKey("location") && payload["location"] != null)
            {
                location = (string)payload["location"];
            }

            if (payload.ContainsKey("start_date") && payload["start_date"] != null)
            {
                startTime = (string)payload["start_date"];
            }

            if (payload.ContainsKey("start_time") && payload["start_time"] != null)
            {
                string startTimeString =  (string)payload["start_time"];

                startTime = (!String.IsNullOrEmpty(startTime)) ? String.Format("{0} {1}", startTime, startTimeString) : startTimeString;
            }

            if (payload.ContainsKey("end_date") && payload["end_date"] != null && payload.ContainsKey("end_time") && payload["end_time"] != null)
            {
                endTime = String.Format("{0} {1}", (string)payload["end_date"], (string)payload["end_time"]);
            }

            SaveAppointmentTask cal = new SaveAppointmentTask();

            cal.Location = location;
            cal.Subject = (string)payload["subject"];
            cal.Details = String.Empty;

            if (!String.IsNullOrEmpty(startTime))
            {
                cal.StartTime = DateTime.Parse(startTime);
            }

            if (!String.IsNullOrEmpty(endTime))
            {
                cal.EndTime = DateTime.Parse(endTime);
            }

            cal.Show();
        }

        // TODO: get appointment(s)

        public async void GetDirections(Dictionary<string, object> payload)
        {
            try
            {
                if (payload.ContainsKey("to"))
                {
                    string destination = (string)payload["to"];

                    LocationModel currLocation = await plexiService.GetDeviceInfo();

                    //IList<MapLocation> locations = await MapService.Default.GeoQuery(destination);

                    var task = new BingMapsDirectionsTask();

                    task.Start = new LabeledMapLocation(null, new System.Device.Location.GeoCoordinate(currLocation.geoCoordinate.Latitude, currLocation.geoCoordinate.Longitude));
                    task.End = new LabeledMapLocation(destination, null);

                    task.Show();
                    return;

                    // show no location message
                    // Messenger.Default.Send(new ShowMessage(errorMessage, errorMessage));
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        public void ShowClock(Dictionary<string, object> payload)
        {
            Debug.WriteLine("show clock");

            ClockViewModel clock = new ClockViewModel();

            if (payload.Count > 0 && payload.ContainsKey("location"))
            {
                JObject location = (JObject)payload["location"];

                JToken value;

                if (location.TryGetValue("dst", out value))
                {
                    clock.ObservesDST = (bool)value;
                }

                TimeSpan timeSpan = (location.TryGetValue("time_offset", out value)) ? TimeSpan.FromHours((int)value) : TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);

                clock.SetDateTime(timeSpan);
            }
            else
            {
                // use current time
                clock.Time = DateTime.Now;
                clock.TimeZoneName = TimeZoneInfo.Local.StandardName;
            }

            MainViewModel vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

            vm.AddDialog(DialogOwner.Plexi, String.Format("the time is {0}", clock.Time.ToString("h:mm tt")), DialogType.Complete);

            vm.Items.Add(clock);
        }

        public void SetAlarm(Dictionary<string, object> payload)
        {
            bool isTimeSet = false;
            bool isDateSet = false;

            DateTime? time = null;
            List<string> dates = new List<string>();

            if (payload.ContainsKey("time"))
            {
                string timeString = (string)payload["time"];

                if (!String.IsNullOrEmpty(timeString))
                {
                    time = DateTime.Parse(timeString);
                    isTimeSet = true;
                }
            }

            if (payload.ContainsKey("date"))
            {
                string dateString = (string)payload["date"];

                if (!String.IsNullOrEmpty(dateString))
                {
                    dates.Add(DateTime.Parse(dateString).ToString("dddd"));
                    isDateSet = true;
                }
            }
            /*
            if (isTimeSet && isDateSet)
            {
                AlarmViewModel alarm = new AlarmViewModel();

                alarm.SaveAlarm("alarm", time.Value, dates);

                MainViewModel main = ViewModelLocator.GetServiceInstance<MainViewModel>();

                main.Items.Add(alarm);
            }
            else
            {*/
                AlarmDetailsViewModel details = ViewModelLocator.GetServiceInstance<AlarmDetailsViewModel>();

                details.AlarmTime = time;
                details.AlarmSelectedItems = dates;

                // we need a lil more info before we can set an alarm
                navigationService.NavigateTo(ViewModelLocator.AlarmPageUri);
            ///}
        }

        public void UpdateAlarm(Dictionary<string, object> payload)
        {

        }

        public void SetReminder(Dictionary<string, object> payload)
        {
            ReminderDetailsViewModel vm = ViewModelLocator.GetServiceInstance<ReminderDetailsViewModel>();

            if (payload.ContainsKey("time"))
            {
                string timeString = (string)payload["time"];

                if (!String.IsNullOrEmpty(timeString))
                {
                    vm.ReminderTime = DateTime.Parse(timeString);
                }
            }

            if (payload.ContainsKey("date"))
            {
                string dateString = (string)payload["date"];

                if (!String.IsNullOrEmpty(dateString))
                {
                    vm.ReminderDate = DateTime.Parse(dateString);
                }
            }

            if (payload.ContainsKey("subject"))
            {
                vm.ReminderSubject = (string)payload["subject"];
            }

            navigationService.NavigateTo(ViewModelLocator.ReminderPageUri);
        }

        public void UpdateReminder(Dictionary<string, object> payload)
        {

        }

        #region helpers
        private JArray GetCandidates()
        {
            var candidates = new JArray();

            if (payload.ContainsKey("contact"))
            {
                JObject contact = (JObject)payload["contact"];

                candidates = (JArray)contact.GetValue("candidates");
            }

            return candidates;
        }

        // NOTE: a callback must be used because searchAsync is not awaitable
        private void GetContacts(JArray candidates, Action<List<Contact>, List<string>> callback)
        {
            var contacts = new Contacts();

            List<Contact> contactList = new List<Contact>();
            List<string> unused_tokens = new List<string>();

            bool isLast = false;
            bool found = false;  //first contact found. any other contacts found are assumed to be part of the message

            contacts.SearchCompleted += (s, e) =>
            {
                if (e.Results.Count() > 0)
                {
                    if (found == false)
                    {
                        found = true;
                        foreach (var c in e.Results)
                        {
                            contactList.Add(c);
                        }
                    }
                    else
                    {
                        unused_tokens.Add(e.Filter);
                    }
                }
                else
                {
                    unused_tokens.Add(e.Filter);
                }

                if (isLast == true)
                {
                    callback(contactList, unused_tokens);
                }
            };

            foreach (string candidate in candidates)
            {

                if (candidate == (string)candidates.Last())
                {
                    isLast = true;
                }

                contacts.SearchAsync(candidate, FilterKind.DisplayName, null);
            }
        }

        private void NoContactFound()
        {
            var dialogService = (IDialogService)(App.Current.RootVisual as PhoneApplicationFrame).Content;

            dialogService.ShowMessageBox("I could not find a contact in your address book", "Contact Lookup");
        }

        // pass any needed data to listviewmodel inorder to complete the process??
        // possibly consider a callback with the choosen contact(s) passed back
        private void ShowContactList(List<Contact> contacts)
        {

            /*
            Debug.WriteLine("show contact list. Count " + contacts.Count);

            var view = ViewModelLocator.GetServiceInstance<ChoiceViewModel>();

            //set data
            view.Choices = contacts.ToList<object>();

            navigationService.NavigateTo(ViewModelLocator.ChoicePageUri);
             */
        }

        private void DoEmailTask(Contact contact, List<string> unused_tokens)
        {
            var email = contact.EmailAddresses.First().EmailAddress;

            string subject = null;
            if (payload.ContainsKey("subject") && payload["subject"] != null)
            {
                subject = (string)payload["subject"];
            }

            var message = unused_tokens.Aggregate((i, j) => i + " " + j) + (string)payload["message"];

            DoEmailTask(email, message, subject);
        }

        private void DoEmailTask(string email, string message, string subject = null)
        {
            var task = new EmailComposeTask();

            task.To = email;

            if (subject != null)
            {
                task.Subject = subject;
            }

            task.Body = message;

            task.Show();
        }

        private void DoSmsTask(Contact contact, List<string> unused_tokens)
        {
           
            var phone = contact.PhoneNumbers.First().PhoneNumber;
            var message = unused_tokens.Aggregate((i, j) => i + " " + j) + (string)payload["message"];

            DoSmsTask(phone, message);
        }

        private void DoSmsTask(string phoneNumber, string message)
        {
            var task = new SmsComposeTask();

            task.To = phoneNumber;
            task.Body = message;

            task.Show();
        }

        private void DoPhoneCallTask(Contact contact)
        {
            var displayName = contact.DisplayName;
            var phoneNumber = contact.PhoneNumbers.First().PhoneNumber;

            DoPhoneCallTask(displayName, phoneNumber);
        }

        private void DoPhoneCallTask(string displayName, string phoneNumber)
        {
            var task = new PhoneCallTask();

            task.DisplayName = displayName;
            task.PhoneNumber = phoneNumber;

            task.Show();
        }

        private void GeoQuery(string searchTerm, Action<GeoCoordinate> callback)
        {
            var query = new GeocodeQuery();
            query.GeoCoordinate = new GeoCoordinate(0, 0);
            query.SearchTerm = searchTerm;
            query.MaxResultCount = 5;

            query.QueryCompleted += (s, e) =>
                {
                    // take first vlaue for now.
                    // possibly return all results and show list
                    if (e.Result.Count > 0)
                    {
                        var first = e.Result.FirstOrDefault();

                        var geo = first.GeoCoordinate;

                        callback(geo);
                    }
                };
        }
        #endregion
    }
}
