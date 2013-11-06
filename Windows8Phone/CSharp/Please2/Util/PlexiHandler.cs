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

            plexiService.onChoice += ChoiceHandler;
            plexiService.onError += ErrorHandler;
            plexiService.onProgress += ProgressHandler;
            plexiService.onShow += ShowHandler;
            plexiService.onAct += ActorHandler;
        }

        private void ChoiceHandler(object sender, ChoiceEventArgs e)
        {
            Dictionary<string, object> simple = e.results.show.simple;

            ResourceDictionary templates = ViewModelLocator.ListTemplates;

            List<ChoiceModel> list = ((JArray)simple["list"]).ToObject<List<ChoiceModel>>();

            ListViewModel vm = ViewModelLocator.GetServiceInstance<ListViewModel>();

            vm.ListResults = list.ToList<object>();
            vm.Template = (DataTemplate)templates["choice"];
            vm.Title = (string)simple["text"];

            if (simple.ContainsKey("text"))
            {
                string show = (string)simple["text"];

                string link = null;

                if (simple.ContainsKey("link"))
                {
                    link = (string)simple["link"];
                }

                Messenger.Default.Send(new ShowMessage(show, e.results.speak, link));
            }

            navigationService.NavigateTo(ViewModelLocator.ListResultsPageUri);
        }

        private void ErrorHandler(object sender, ErrorEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show(e.message, "Server Error", MessageBoxButton.OK);

            if (response == MessageBoxResult.OK)
            {
                plexiService.ClearContext();
            }
        }

        private void ProgressHandler(object sender, ProgressEventArgs e)
        {
            Messenger.Default.Send(new ProgressMessage(e.inProgress));
        }

        private void ShowHandler(object sender, ShowEventArgs e)
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

        private void ActorHandler(object sender, ActorEventArgs e)
        {
            ActorModel response = e.actor;

            Uri page = ViewModelLocator.ConversationPageUri;

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
                                        page = uri;
                                    }
                                    break;

                                default:
                                    data = PopulateViewModel("list", structured);

                                    if (data != null)
                                    {
                                        page = ViewModelLocator.ListResultsPageUri;
                                    }
                                    break;
                            }
                            break;

                        case "simple":
                        case "single":
                            uri = LoadSingleResult(structured);

                            if (uri != null)
                            {
                                page = uri;
                            }
                            break;
                    }
                }
            }

            navigationService.NavigateTo(page);
        }

        private Dictionary<string, object> PopulateViewModel(string templateName, Dictionary<string, object> structured)
        {
            try
            {
                ViewModelLocator locator = App.Current.Resources["Locator"] as ViewModelLocator;

                PropertyInfo viewmodelProperty = locator.GetType().GetProperty(templateName.CamelCase() + "ViewModel");

                if (viewmodelProperty == null)
                {
                    Debug.WriteLine("pouplateviewmodel: view model " + templateName + " could not be found");
                    return null;
                }

                object viewModel = viewmodelProperty.GetValue(locator, null);

                MethodInfo populateMethod = viewModel.GetType().GetMethod("Populate");

                if (populateMethod == null)
                {
                    Debug.WriteLine("populateviewmodel: 'Populate' method not implemented in " + templateName);
                    return null;
                }

                if (structured.ContainsKey("items") && ((JArray)structured["items"]).Count <= 0)
                {
                    Debug.WriteLine("populateviewmodel: items list is emtpy nothing to set");
                    return null;
                }

                if (structured.ContainsKey("item") && ((JObject)structured["item"]).Count <= 0)
                {
                    Debug.WriteLine("populateviewmodel: item object is emtpy nothing to set");
                    return null;
                }

                object[] parameters = (templateName == "list") ? new object[] { structured } : new object[] { templateName, structured };

                return (Dictionary<string, object>)populateMethod.Invoke(viewModel, parameters);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return null;
            }
        }

        private Uri LoadSingleResult(Dictionary<string, object> structured)
        {
            ResourceDictionary templates = ViewModelLocator.SingleTemplates;

            Uri page = ViewModelLocator.SingleResultPageUri;

            string[] template = (structured["template"] as string).Split(':');

            string type = template[0];
            string templateName = template[1];

            if (template.Count() > 2)
            {
                templateName += ":" + template[2];
            }

            if (templates[templateName] == null)
            {
                Debug.WriteLine("single template not found: " + templateName);
                return null;
            }

            SingleViewModel singleViewModel = ViewModelLocator.GetServiceInstance<SingleViewModel>();

            singleViewModel.ContentTemplate = (DataTemplate)templates[templateName];

            Dictionary<string, object> data = PopulateViewModel(template[1], structured);

            if (data == null)
            {
                return null;
            }

            if (data.ContainsKey("page"))
            {
                page = (Uri)data["page"];
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

            return page;
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
