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

using Please2.ViewModels;

using Plexi.Models;
namespace Please2.Util
{
    class TaskService : ITaskService
    {
        private Dictionary<string, object> payload;

        private INavigationService navigationService;

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

        /*
        private string taskToFulfill;
        public string TaskToFulfill
        {
            get
            {
                return taskToFulfill;
            }
        }
        */

        public TaskService()
        {
            navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();
        }

        public void ComposeEmail(Dictionary<string, object> payload)
        {

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

        public void ComposeSms(Dictionary<string, object> payload)
        {
            if (payload.ContainsKey("contact"))
            {
                var contact = payload["contact"] as JObject;

                if (contact.GetValue("phone_numbers") != null)
                {
                    var numbers = contact["phone_numbers"] as JArray;

                    if (numbers.Count > 0)
                    {
                        var primary = numbers.First();


                    }

                }
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

        public void PhoneCall(Dictionary<string, object> payload)
        {

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

        public void SetAppointment()
        {
            var cal = new SaveAppointmentTask();

            cal.Subject = (string)payload["subject"];
            cal.Details = "";
            if (payload.ContainsKey("location") && payload["location"] != null)
            {
                cal.Location = (string)payload["location"];
            }
            cal.StartTime = DateTime.Now;
            if (payload.ContainsKey("end_date") && payload["end_date"] != null)
            {
                cal.EndTime = DateTime.Parse((string)payload["end_date"]);
            }

            cal.Show();
        }

        // TODO: get appointment(s)

        public void GetDirections()
        {
            var dir = new MapsDirectionsTask();

            dir.Start = new LabeledMapLocation("start text", new System.Device.Location.GeoCoordinate(33.4930947, -111.928558)); // scottsdale
            dir.End = new LabeledMapLocation("end text", new System.Device.Location.GeoCoordinate(33.4930947, -111.928558)); // scottsdale

            dir.Show();
        }

        public void ShowClock()
        {
            navigationService.NavigateTo(ViewModelLocator.TimePageUri);
        }

        public void SetAlarm()
        {
            // need to prepop view model
            navigationService.NavigateTo(ViewModelLocator.AlarmPageUri);
        }

        public void SetReminder()
        {
            // need to prepop view model
            navigationService.NavigateTo(ViewModelLocator.ReminderPageUri);
        }

        #region helpers
        private JArray GetCandidates()
        {
            var candidates = new JArray();

            if (payload.ContainsKey("contact"))
            {
                var contact = (Newtonsoft.Json.Linq.JObject)payload["contact"];

                candidates = (Newtonsoft.Json.Linq.JArray)contact.GetValue("candidates");
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
            Debug.WriteLine("show contact list. Count " + contacts.Count);

            var listView = ViewModelLocator.GetServiceInstance<ListViewModel>();

            //set data
            listView.ListResults = contacts.ToList<object>();

            navigationService.NavigateTo(ViewModelLocator.ListResultsPageUri);
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
