using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;

using Microsoft.Phone.Controls;

using Newtonsoft.Json;

using Please.Models;

namespace Please.ViewModels
{
    public class PleaseViewModel: ViewModelBase
    {
        // list of spoken and response text
        private ObservableCollection<DialogModel> _pleaseList;
        public ObservableCollection<DialogModel> PleaseList
        {
            get
            {
                return _pleaseList;
            }
            set
            {
                _pleaseList = value;
                NotifyPropertyChanged("PleaseList");
            }
        }

        public void AddDialog(string sender, object message, string template = null)
        {
            if (PleaseList == null)
            {
                PleaseList = new ObservableCollection<DialogModel>();
            }

            DataTemplate dataTemplate;

            var res = ((Application.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).Resources;

            if (template == null)
            {
                template = "DefaultDataTemplate";
            }

            if (res.Contains(template))
            {
                dataTemplate = (DataTemplate) res[template];
            }
            else
            {
                // throw exception??
                Debug.WriteLine("could not find the search template. loading empty DataTemplate");

                dataTemplate = new DataTemplate();
            }
         
            var dialog = new DialogModel();

            dialog.sender = sender;
            dialog.message = message;
            dialog.template = dataTemplate;

            PleaseList.Add(dialog);
        }

        /// <summary>
        /// A test method to add template results to the local DB to reduce calls out to the API.
        /// <param name="output"></param>
        /// </summary>
        public void AddOutput(string output)
        {
            try
            {
                PleaseContext db;

                String connectionString = "Data Source=isostore:/Please.sdf";

                using (db = new PleaseContext(connectionString))
                {

                    Random rndm = new Random();

                    if (db.DatabaseExists() == false)
                    {
                        db.CreateDatabase();
                    }

                    db.Outputs.InsertOnSubmit(
                        new OutputItem
                        {
                            ID = rndm.Next(1, 100),
                            Value = output
                        }
                    );

                    db.SubmitChanges();
                }
            }
            catch (System.Data.DataException dbErr)
            {
                Debug.WriteLine(dbErr.Message);
                Debug.WriteLine(dbErr.Data.ToString());
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        /// <summary>
        /// A test method to get the most recent record containing template results retrieved from the API.
        /// <param name="output"></param>
        /// </summary>
        public string GetOutput()
        {
            PleaseContext db;

            String connectionString = "Data Source=isostore:/Please.sdf";

            using (db = new PleaseContext(connectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    return null;
                }

                var count = db.Outputs.Count<OutputItem>();

                if (count > 0)
                {
                    var item = db.Outputs.ElementAt<OutputItem>(count - 1);
                    //var item = db.Outputs.First<OutputItem>();

                    return item.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// A test method to populate the dialog screen with some arbitrary data.
        /// </summary>
        public void addDialogItems(int dialogCount = 4)
        {
            string currentDialogUser = "user";

            for (var i = 0; i < dialogCount; i++)
            {
                string text = ("testing line " + (i + 1)).ToString();

                App.PleaseViewModel.AddDialog(currentDialogUser, text);

                currentDialogUser = (currentDialogUser.Equals("user")) ? "please" : "user";
            }
        }

        public bool addDialogTemplate(string template = "ShoppingDataTemplate")
        {
            var testOutput = GetOutput();

            Debug.WriteLine(testOutput);

            if (String.IsNullOrEmpty(testOutput) == false)
            {
                string output = Convert.ToString(testOutput);

                var jsonSettings = new JsonSerializerSettings();

                jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                jsonSettings.NullValueHandling = NullValueHandling.Include;

                var showOutput = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShoppingModel>>(output, jsonSettings);
      
                AddDialog("please", showOutput, template);

                return true;
            }

            return false;
        }
    }
}
