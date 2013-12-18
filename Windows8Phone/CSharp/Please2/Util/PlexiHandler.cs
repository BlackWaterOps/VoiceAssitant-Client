using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;

using GalaSoft.MvvmLight.Messaging;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.ViewModels;

using Plexi;
using Plexi.Events;
using Plexi.Models;
using Plexi.Util;

// NOTE: currently this object is instantiated in the view locator, 
// but really all this logic is better suited as a view model base
// which means handling adding and releasing the event handlers :(
namespace Please2.Util
{
    class PlexiHandler
    {
        INavigationService navigationService;

        IPlexiService plexiService;

        public PlexiHandler()
        {
            Debug.WriteLine("plexi handler initialized");
            // attach navigation service
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            // attach plexi service
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            plexiService.Choose += OnChoose;
            plexiService.Error += OnError;
            plexiService.Register += OnRegister;
            plexiService.InProgress += OnProgress;
            plexiService.Show += OnShow;
            plexiService.Act += OnAct;
        }

        private void Show(ShowModel model, string speak = "")
        {
            Dictionary<string, object> simple = model.simple;

            if (simple.ContainsKey("text"))
            {
                string show = (string)simple["text"];

                string link = null;

                if (simple.ContainsKey("link"))
                {
                    link = (string)simple["link"];
                }

                Messenger.Default.Send(new ShowMessage(show, speak, link));
            }
        }

        private void OnChoose(object sender, ChoiceEventArgs e)
        {
            Dictionary<string, object> simple = e.results.show.simple;

            ResourceDictionary templates = ViewModelLocator.ListTemplates;

            List<ChoiceModel> list = ((JArray)simple["list"]).ToObject<List<ChoiceModel>>();

            ListViewModel vm = ViewModelLocator.GetServiceInstance<ListViewModel>();

            vm.ListResults = list.ToList<object>();
            vm.Template = (DataTemplate)templates["choice"];
            vm.Title = (string)simple["text"];

            Show(e.results.show, e.results.speak);
           
            navigationService.NavigateTo(ViewModelLocator.ListResultsPageUri);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show(e.message, "Server Error", MessageBoxButton.OK);

            if (response == MessageBoxResult.OK)
            {
                plexiService.ClearContext();
            }
        }

        //TODO: switch messaging 
        private void OnRegister(object sender, RegisterEventArgs e)
        {
            string model = e.model.Split('_')[0];

            string message = String.Empty;

            switch (model)
            {
                case "email":
                case "sms":
                case "call":
                    message = "Would you like to setup a Stremor account so all your information can be stored in the cloud?";
                    break;

                case "fitness":
                case "food":
                case "facebook":
                    message = "Please create a Stremor account to continue";
                    break;
            }

            MessageBoxResult response = MessageBox.Show(message, "Account Registration", MessageBoxButton.OKCancel);

            if (response.Equals(MessageBoxResult.OK))
            {
                // navigate to registration page
            }
            else if (response.Equals(MessageBoxResult.Cancel))
            {
                // 1. set flag in plexi service so this event isn't raised every time personal user data is needed.
                // 2. the user will have to go into the settings views afterwards to setup a stremor account which 
                //    will unlock the ability to auth accounts like google, facebook, fitbit
                // 3. search local 
            }
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            try
            {
                Messenger.Default.Send(new ProgressMessage(e.inProgress));
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("OnProgress Error: {0}", err.Message));
            }
        }

        private void OnShow(object sender, ShowEventArgs e)
        {
            Messenger.Default.Send(new ShowMessage(e.show, e.speak, e.link));

            if (e.status == "inprogress")
            {
                PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;

                if (!frame.CurrentSource.Equals(ViewModelLocator.ConversationPageUri))
                {
                    this.navigationService.NavigateTo(ViewModelLocator.ConversationPageUri);
                }
            }
        }

        private void OnAct(object sender, ActorEventArgs e)
        {
            try
            {
                ActorModel response = e.actor;

                Show(response.show, response.speak);

                Uri view = ViewModelLocator.ConversationPageUri;

                if (response.show.structured != null && response.show.structured.ContainsKey("template"))
                {
                    Dictionary<string, object> structured = response.show.structured;

                    string[] template = (structured["template"] as string).Split(':');

                    string type = template[0];
                    string templateName = null;

                    if (template.Count() > 1)
                    {
                        templateName = template[1];
                    }

                    if (templateName != null)
                    {
                        Dictionary<string, object> data;

                        ResourceDictionary templates;

                        Uri uri;

                        switch (type)
                        {
                            case "list":
                                templates = ViewModelLocator.ListTemplates;

                                switch (templateName)
                                {
                                    // override. list items that act like single items
                                    case "news":
                                        uri = LoadSingleResult(structured);

                                        if (uri != null)
                                        {
                                            view = uri;
                                        }
                                        break;

                                    default:
                                        data = PopulateViewModel("list", structured);
                             
                                        if (data != null)
                                        {
                                            view = ViewModelLocator.ListResultsPageUri;
                                        }
                                        break;
                                }
                                break;

                            case "simple":
                            case "single":
                                uri = LoadSingleResult(structured);

                                if (uri != null)
                                {
                                    view = uri;
                                }
                                break;
                        }
                    }
                }

                navigationService.NavigateTo(view);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("OnAct Error: {0}", err.Message));
            }
        }

        private Dictionary<string, object> PopulateViewModel(string templateName, Dictionary<string, object> structured)
        {
            try
            {
                ViewModelLocator locator = App.Current.Resources["Locator"] as ViewModelLocator;

                PropertyInfo viewmodelProperty = locator.GetType().GetProperty(String.Format("{0}ViewModel", templateName.CamelCase()));

                if (viewmodelProperty == null)
                {
                    Debug.WriteLine(String.Format("PouplateViewModel: view model {0} could not be found", templateName));
                    return null;
                }

                object viewModel = viewmodelProperty.GetValue(locator, null);

                MethodInfo populateMethod = viewModel.GetType().GetMethod("Populate");

                if (populateMethod == null)
                {
                    Debug.WriteLine(String.Format("PopulateViewModel: 'Populate' method not implemented in {0}", templateName));
                    return null;
                }

                if (!structured.ContainsKey("item") && !structured.ContainsKey("items"))
                {
                    Debug.WriteLine("PopulateViewModel: unable to find 'item' or 'items' in response");
                    return null;
                }

                if (structured.ContainsKey("items") && ((JArray)structured["items"]).Count <= 0)
                {
                    Debug.WriteLine("PopulateViewModel: items list is emtpy nothing to set");
                    return null;
                }

                if (structured.ContainsKey("item") && ((JObject)structured["item"]).Count <= 0)
                {
                    Debug.WriteLine("PopulateViewModel: item object is emtpy nothing to set");
                    return null;
                }

                object[] parameters = (templateName == "list") ? new object[] { structured } : new object[] { templateName, structured };
                
                return (Dictionary<string, object>)populateMethod.Invoke(viewModel, parameters);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("PopulateViewModel Error: {0}", err.Message));
                return null;
            }
        }

        private Uri LoadSingleResult(Dictionary<string, object> structured)
        {
            try
            {
                ResourceDictionary templates = ViewModelLocator.SingleTemplates;

                Uri view = ViewModelLocator.SingleResultPageUri;

                string[] template = (structured["template"] as string).Split(':');

                string type = template[0];
                string templateName = template[1];

                Debug.WriteLine(type);
                Debug.WriteLine(templateName);

                if (template.Count() > 2)
                {
                    templateName += ":" + template[2];
                }

                if (templates[templateName] == null)
                {
                    Debug.WriteLine(String.Format("single template not found: {0}", templateName));
                    return null;
                }
               
                SingleViewModel singleViewModel = ViewModelLocator.GetServiceInstance<SingleViewModel>();

                singleViewModel.ContentTemplate = (DataTemplate)templates[templateName];

                Dictionary<string, object> data = PopulateViewModel(template[1], structured);

                if (data == null)
                {
                    return null;
                }

                // allow viewmodel to override default view. Currently not in use! 
                if (data.ContainsKey("page"))
                {
                    view = (Uri)data["page"];
                }

                singleViewModel.Title = null;

                if (data.ContainsKey("title"))
                {
                    singleViewModel.Title = (string)data["title"];
                }

                singleViewModel.SubTitle = null;

                if (data.ContainsKey("subtitle"))
                {
                    singleViewModel.SubTitle = (string)data["subtitle"];
                }

                if (data.ContainsKey("scheme"))
                {
                    singleViewModel.Scheme = (string)data["scheme"];
                }

                return view;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("LoadSingleResult Error: {0}", err.Message));
                return null;
            }
        }

        /*
        private void ActorInterceptor(string actor)
        {
            Dictionary<string, object> payload = this.mainContext.payload;

            if (!SimpleIoc.Default.IsRegistered<ITaskService>())
            {
                SimpleIoc.Default.Register<ITaskService, TaskService>();
            }

            ITaskService tasks = SimpleIoc.Default.GetInstance<ITaskService>();

            tasks.MainContext = this.mainContext;

            switch (actor)
            {
                case "time":
                    tasks.ShowClock();
                    break;

                case "email":
                case "email_denial":
                    tasks.ComposeEmail(payload);
                    break;

                case "sms":
                case "sms_denial":
                    tasks.ComposeSms(payload);
                    break;

                case "directions":
                case "directions_denial":
                    tasks.GetDirections();
                    break;

                case "call":
                case "call_denial":
                    tasks.PhoneCall(payload);
                    break;

                case "calendar":
                case "calendar_create":
                case "calendar_create_denial":
                    tasks.SetAppointment();
                    break;

                case "information":

                    break;

                case "alarm":
                    tasks.SetAlarm();
                    break;

                case "reminder":
                    tasks.SetReminder();
                    break;
            }

            return;
        }
        */
    }
}
