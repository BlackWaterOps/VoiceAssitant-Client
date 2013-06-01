using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.UserData;

using Windows.Phone.Networking.NetworkOperators;

using Please.Models;
// <Capability Name="ID_CAP_SMS_INTERCEPT_AGENT"/>
// <Capability Name="ID_CAP_SMS_INTERCEPT_RECIPIENT"/>
// 
namespace Please.Util
{
    public class Extractor
    {
        //IEnumerable<Appointment>AppointmentResults;
        //IEnumerable<Contact>ContactResults;

        private PleaseContext pleaseDB;
        const string pleaseConnectionString = "Data Source=isostore:/Please.sdf";

        public void Extract()
        {
            SmsInterceptor.SmsReceived += SmsInterceptor_SmsReceived;

            System.DateTime startDate;

            System.DateTime endDate;

            System.DateTime now = DateTime.Now;

            using (pleaseDB = new PleaseContext(pleaseConnectionString))
            {
                if (pleaseDB.DatabaseExists() == false)
                {
                    // contacts
                    GetContacts();

                    // get appointents in the past with a max of 200 appointments
                    endDate = now;

                    startDate = now.AddYears(-1);

                    GetAppointments(startDate, endDate, 200);

                    // get appointents in the past with a max of 200 appointments
                    startDate = now;

                    endDate = now.AddYears(2);

                    GetAppointments(startDate, endDate, 500);
                }
                else
                {
                    if (pleaseDB.Contacts.Count() == 0)
                    {
                        GetContacts();
                    }

                    if (pleaseDB.Appointments.Count() == 0)
                    {
                        // get appointents in the past with a max of 200 appointments
                        endDate = now;

                        startDate = now.AddYears(-1);

                        GetAppointments(startDate, endDate, 200);

                        // get appointents in the past with a max of 200 appointments
                        startDate = now;

                        endDate = now.AddYears(2);

                        GetAppointments(startDate, endDate, 500);
                    }
                }
            }
        }

        public void GetAppointments(DateTime startDate, DateTime endDate, int maxAppointments = 100)
        {
            var appts = new Appointments();

            appts.SearchCompleted += new EventHandler<AppointmentsSearchEventArgs>(AppointmentsSearchComplete);

            appts.SearchAsync(startDate, endDate, maxAppointments, "");
        }

        public void GetContacts()
        {
            var contacts = new Contacts();

            contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(ContactsSearchComplete);

            contacts.SearchAsync(String.Empty, FilterKind.None, "");
        }

        void AppointmentsSearchComplete(object sender, AppointmentsSearchEventArgs e)
        {
            try
            {
                foreach (Appointment appointment in e.Results)
                {
                    // int id = Appointment.
                    int hash = appointment.GetHashCode();
                }

                // add to DB
                //this.sendDataToServer(AppointmentResults);
            }
            catch (System.Exception)
            {
                //
            }
        }

        void ContactsSearchComplete(object sender, ContactsSearchEventArgs e)
        {
            try
            {
                foreach (Contact contact in e.Results)
                {
                    //int id = 
                    int hash = contact.GetHashCode();
                }
                
                // send results to sever
                //this.sendDataToServer(ContactResults);
            }
            catch (System.Exception)
            {
                // 
            }
        }

        void SmsInterceptor_SmsReceived(object sender, object e)
        {
            var message = (e as InterceptedSmsMessage);

            throw new NotImplementedException();
        }

        void sendDataToServer(object data)
        {
            // var req = new Please.Util.Request();

            // req.Method = "POST";
            // req.ContentType = "text/json";

            // req.DoRequestJsonAsync<`insert model here`>(`insert endpoint`, `insert data here`);
        }
    }
}
