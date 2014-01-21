using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;

using Windows.System;

using GalaSoft.MvvmLight.Messaging;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.ViewModels;

using PlexiSDK;
using PlexiSDK.Events;
using PlexiSDK.Models;
using PlexiSDK.Util;

// NOTE: currently this object is instantiated in the view locator, 
// but really all this logic is better suited as a view model base
// which means handling adding and releasing the event handlers :(
namespace Please2.Util
{
    /// <summary>
    /// Specifies the actors handled locally
    /// </summary>
    public enum Actor
    {
        Call,
        Email,
        Sms,
        Directions,
        Calendar,
        Alarm,
        Reminder,
        Time
    }

    class PlexiHandler
    {
        INavigationService navigationService;

        IPlexiService plexiService;

        private static Dictionary<string, Actor> ActorMap = new Dictionary<string, Actor>()
        {
            {"alarm", Actor.Alarm},
            {"calendar", Actor.Calendar},
            {"call", Actor.Call},
            {"directions", Actor.Directions},
            {"email", Actor.Email},
            {"reminder", Actor.Reminder},
            {"sms", Actor.Sms},
            {"time", Actor.Time}
        };

        private static Dictionary<Tuple<Actor, string>, Delegate> Actors = new Dictionary<Tuple<Actor, string>, Delegate>();

        public static readonly PlexiHandler Default = new PlexiHandler();

        private PlexiHandler()
        {
            // attach navigation service
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();

            // attach plexi service
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            BuildActors();
        }

        private void BuildActors()
        {
            if (Actors.Count == 0)
            {
                ITaskService tasks = ViewModelLocator.GetServiceInstance<ITaskService, TaskService>();

                Actors.Add(new Tuple<Actor, string>(Actor.Alarm, "create"), new Action<Dictionary<string, object>>(tasks.SetAlarm));
                Actors.Add(new Tuple<Actor, string>(Actor.Alarm, "edit"), new Action<Dictionary<string, object>>(tasks.UpdateAlarm));

                Actors.Add(new Tuple<Actor, string>(Actor.Reminder, "create"), new Action<Dictionary<string, object>>(tasks.SetReminder));
                Actors.Add(new Tuple<Actor, string>(Actor.Reminder, "edit"), new Action<Dictionary<string, object>>(tasks.UpdateReminder));

                Actors.Add(new Tuple<Actor, string>(Actor.Time, "create"), new Action<Dictionary<string, object>>(tasks.ShowClock));

                Actors.Add(new Tuple<Actor, string>(Actor.Email, "create"), new Action<Dictionary<string, object>>(tasks.ComposeEmail));

                Actors.Add(new Tuple<Actor, string>(Actor.Sms, "create"), new Action<Dictionary<string, object>>(tasks.ComposeSms));

                Actors.Add(new Tuple<Actor, string>(Actor.Directions, "query"), new Action<Dictionary<string, object>>(tasks.GetDirections));

                Actors.Add(new Tuple<Actor, string>(Actor.Call, "trigger"), new Action<Dictionary<string, object>>(tasks.PhoneCall));

                Actors.Add(new Tuple<Actor, string>(Actor.Calendar, "create"), new Action<Dictionary<string, object>>(tasks.SetAppointment));
            }
        }

        public void Listen()
        {
            plexiService.Choose += OnChoose;
            plexiService.Error += OnError;
            plexiService.Authorize += OnAuthorize;
            plexiService.InProgress += OnProgress;
            plexiService.Show += OnShow;
            plexiService.Act += OnAct;
            plexiService.Complete += OnComplete;
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

            ChoiceViewModel vm = new ChoiceViewModel();

            vm.Load(null, simple);

            SingleViewModel svm = ViewModelLocator.GetServiceInstance<SingleViewModel>();

            svm.Content = vm;
            svm.Title = (string)simple["text"];

            Show(e.results.show, e.results.speak);
           
            navigationService.NavigateTo(ViewModelLocator.SingleResultPageUri);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show(e.message, "Server Error", MessageBoxButton.OK);

            if (response == MessageBoxResult.OK)
            {
                plexiService.ClearContext();
            }
        }

        private void OnAuthorize(object sender, AuthorizationEventArgs e)
        {
            string message;

            if (e.model != null)
            {
                string model = e.model.Split('_')[0];

                message = String.Format("Oops, it looks like we don't have an account synced for {0}. Please sync an account to continue.", model);
            }
            else
            {
                message = "Oops, it looks like we don't have an account synced. Please sync an account to continue.";
            }

            MessageBoxResult response = MessageBox.Show(message, "Account Authorization", MessageBoxButton.OKCancel);

            if (response.Equals(MessageBoxResult.OK))
            {
                // navigate to settings page to sync various accounts
                navigationService.NavigateTo(ViewModelLocator.SettingsPageUri);

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

            if (e.status == State.InProgress)
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
            ClassifierModel data = e.data;

            string action = data.action;

            // run local actors
            if (ActorMap.ContainsKey(data.model))
            {
                ITaskService tasks = ViewModelLocator.GetServiceInstance<ITaskService, TaskService>();

                Actor actor = ActorMap[e.data.model];

                Dictionary<string, object> payload = e.data.payload;

                if (Actors != null && Actors.Count > 0)
                {
                    Tuple<Actor, string> actorAction = new Tuple<Actor, string>(actor, action);

                    if (Actors.ContainsKey(actorAction))
                    {
                        e.handled = true;
                        Actors[actorAction].DynamicInvoke(payload);
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("action'{0}' is not supported by {1}", action, actor));
                    }
                }
                else
                {
                    Debug.WriteLine("no actors set up. nothing to do");
                }
            }
        }

        private void OnComplete(object sender, CompleteEventArgs e)
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

                    bool viewModelLoaded = false;

                    object viewModel = null;

                    // try to load specific viewmodel. ie. shopping:amazon
                    if (template.Count() > 2)
                    {
                        string secondaryString = template[2];

                        // normalize fitbit secondary names
                        switch (template[2])
                        {
                            case "weight":
                                secondaryString = "weight";
                                break;

                            case "food":
                            case "log-food":
                            case "calories":
                                secondaryString = "food";
                                break;

                            case "activity":
                            case "log-activity":
                                secondaryString = "activity";
                                break;
                        }

                        templateName = String.Format("{0}:{1}", template[1], secondaryString);

                        viewModelLoaded = TryLoadViewModel(templateName, structured, out viewModel);
                    }

                    // if a specific vm was not found, try to load up a generic one. ie. shopping
                    if (!viewModelLoaded && template.Count() > 1)
                    {
                        templateName = template[1];

                        viewModelLoaded = TryLoadViewModel(templateName, structured, out viewModel);
                    }

                    if (viewModelLoaded)
                    {
                        view = ViewModelLocator.SingleResultPageUri;

                        SingleViewModel resultsPage = ViewModelLocator.GetServiceInstance<SingleViewModel>();
                        resultsPage.Content = viewModel;
                    }
                }

                navigationService.NavigateTo(view);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        /*
        private void OnComplete(object sender, CompleteEventArgs e)
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
        */

        private bool TryLoadViewModel(string templateName, Dictionary<string, object> structured, out object viewModel)
        {
            try
            {
                /*
                ViewModelLocator locator = App.Current.Resources["Locator"] as ViewModelLocator;

                PropertyInfo viewmodelProperty = locator.GetType().GetProperty(String.Format("{0}ViewModel", templateName.CamelCase()));

                if (viewmodelProperty == null)
                {
                    Debug.WriteLine(String.Format("LoadViewModel: view model {0} could not be found", templateName));
                    viewModel = null;
                    return false;
                }

                object vm = viewmodelProperty.GetValue(locator, null);
                */

                string viewModelName = String.Format("{0}ViewModel", templateName.CamelCase());

                Debug.WriteLine(Assembly.GetExecutingAssembly().GetName().Name);

                string fullTypeName = String.Format("{0}.ViewModels.{1}", Assembly.GetExecutingAssembly().GetName().Name, viewModelName);

                Type type = Type.GetType(fullTypeName);

                if (type == null)
                {
                    Debug.WriteLine(String.Format("TryLoadViewModel: view model {0} could not be found", templateName));
                    viewModel = null;
                    return false;
                }

                object vm = Activator.CreateInstance(type);

                MethodInfo loadMethod = vm.GetType().GetMethod("Load");

                if (loadMethod == null)
                {
                    Debug.WriteLine(String.Format("TryLoadViewModel: 'Populate' method not implemented in {0}", templateName));
                    viewModel = null;
                    return false;
                }

                if (!structured.ContainsKey("item") && !structured.ContainsKey("items"))
                {
                    Debug.WriteLine("TryLoadViewModel: unable to find 'item' or 'items' in response");
                    viewModel = null;
                    return false;
                }

                if (structured.ContainsKey("items") && ((JArray)structured["items"]).Count <= 0)
                {
                    Debug.WriteLine("TryLoadViewModel: items list is emtpy nothing to set");
                    viewModel = null;
                    return false;
                }

                if (structured.ContainsKey("item") && ((JObject)structured["item"]).Count <= 0)
                {
                    Debug.WriteLine("TryLoadViewModel: item object is emtpy nothing to set");
                    viewModel = null;
                    return false;
                }

                //object[] parameters = (templateName == "list") ? new object[] { structured } : new object[] { templateName, structured };
                object[] parameters = new object[] { templateName, structured };

                Dictionary<string, object> response = (Dictionary<string, object>)loadMethod.Invoke(vm, parameters);

                // TODO: return the dict along with the vm
                SingleViewModel svm = ViewModelLocator.GetServiceInstance<SingleViewModel>();

                if (response.ContainsKey("scheme"))
                {
                    svm.Scheme = (ColorScheme)response["scheme"];
                }

                if (response.ContainsKey("title"))
                {
                    svm.Title = (string)response["title"];
                }

                if (response.ContainsKey("subtitle"))
                {
                    svm.SubTitle = (string)response["subtitle"];
                }

                viewModel = vm;

                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("PopulateViewModel Error: {0}", err.Message));
                viewModel = null;
                return false;
            }
        }

        /*
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
                    //Debug.WriteLine(data["scheme"].GetType());
                    singleViewModel.Scheme = (ColorScheme)data["scheme"];
                }

                return view;
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("LoadSingleResult Error: {0}", err.Message));
                return null;
            }
        }
        */
    }
}
