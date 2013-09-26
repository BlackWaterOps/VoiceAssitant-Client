using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Ioc;

using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;

using Please2.Models;
using Please2.ViewModels;

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
            navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
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

        public void ComposeSms(Contact contact)
        {
            DoSmsTask(contact, this.unused_tokens);
        }

        public void ComposeSms()
        {
            var candidates = GetCandidates();

            if (candidates.Count > 0)
            {
                GetContacts(candidates, (contactList, unused_tokens) =>
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
                });
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

        public void GetDirections()
        {
            var dir = new MapsDirectionsTask();

            dir.Start = new LabeledMapLocation("start text", new System.Device.Location.GeoCoordinate(33.4930947, -111.928558)); // scottsdale
            dir.End = new LabeledMapLocation("end text", new System.Device.Location.GeoCoordinate(33.4930947, -111.928558)); // scottsdale

            dir.Show();
        }

        public void ShowClock()
        {
            navigationService.NavigateTo(new Uri("/Views/Clock.xaml", UriKind.Relative));
        }

        #region helpers
        private Newtonsoft.Json.Linq.JArray GetCandidates()
        {
            var candidates = new Newtonsoft.Json.Linq.JArray();

            if (payload.ContainsKey("contact"))
            {
                var contact = (Newtonsoft.Json.Linq.JObject)payload["contact"];

                candidates = (Newtonsoft.Json.Linq.JArray)contact.GetValue("candidates");
            }

            return candidates;
        }

        // NOTE: a callback must be used because searchAsync is not awaitable
        private void GetContacts(Newtonsoft.Json.Linq.JArray candidates, Action<List<Contact>, List<string>> callback)
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

        // pass any needed data to listviewmodel inorder to complete the process??
        // possibly consider a callback with the choosen contact(s) passed back
        private void ShowContactList(List<Contact> contacts)
        {
            if (!SimpleIoc.Default.IsRegistered<ListViewModel>())
            {
                SimpleIoc.Default.Register<ListViewModel>();
            }

            var listView = SimpleIoc.Default.GetInstance<ListViewModel>();

            //set data
            listView.ListResults = contacts.ToList<object>();

            navigationService.NavigateTo(new Uri("/Views/ListResults.xaml", UriKind.Relative));
        }

        private void DoEmailTask(Contact contact, List<string> unused_tokens)
        {
            var task = new EmailComposeTask();

            task.To = contact.EmailAddresses.First().EmailAddress;

            if (payload.ContainsKey("subject") && payload["subject"] != null)
            {
                task.Subject = (string)payload["subject"];
            }

            task.Body = unused_tokens.Aggregate((i, j) => i + " " + j) + (string)payload["message"];

            task.Show();
        }

        private void DoSmsTask(Contact contact, List<string> unused_tokens)
        {
            var task = new SmsComposeTask();

            /*
            var contact = (Newtonsoft.Json.Linq.JObject)payload["contact"];
            var numbers = (Newtonsoft.Json.Linq.JArray)contact.GetValue("phone_numbers");
            var number = (Newtonsoft.Json.Linq.JObject)numbers.First;
            */

            task.To = contact.PhoneNumbers.First().PhoneNumber;
            task.Body = unused_tokens.Aggregate((i, j) => i + " " + j) + (string)payload["message"];

            task.Show();
        }
        
        private void DoPhoneCallTask(Contact contact)
        {
            var task = new PhoneCallTask();

            task.DisplayName = contact.DisplayName;
            task.PhoneNumber = contact.PhoneNumbers.First().PhoneNumber;

            task.Show();
        }


        #endregion
    }
}
