using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class MainMenuViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private int columns = 2;
        public int Columns
        {
            get { return columns; }
        }

        private int margin = 5;
        public int Margin
        {
            get { return margin; }
        }

        public string SubTitle { get { return DateTime.Now.ToString("dddd, MMMM d, yyyy @ h:mm tt"); } }

        private ObservableCollection<MainMenuModel> mainMenu;
        public ObservableCollection<MainMenuModel> MainMenu
        {
            get { return mainMenu; }
            set 
            {
                mainMenu = value;
                RaisePropertyChanged("MainMenu");
            }
        }

        public Size GridCellSize
        {
            get
            {
                return new Size(221.5, 221.5);
            }
        }

        public RelayCommand MenuLoaded { get; set; }

        private INavigationService navigationService;

        public MainMenuViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;

            MenuLoaded = new RelayCommand(SetGridSize);

            AddDefaultMenuItems();

            //GeoQueryTest();
        }

        /* for testing
        private async void GeoQueryTest()
        {
            Debug.WriteLine("get geoquery");

            IList<MapLocation> locations = await MapService.GeoQuery("");

            if (locations.Count > 0)
            {
                foreach (MapLocation location in locations)
                {
                    Debug.WriteLine(String.Format("location: {0} {1}", location.GeoCoordinate.Latitude, location.GeoCoordinate.Longitude));
                }
            }
            else
            {
                Debug.WriteLine("no locations returned");
            }
        }
        */
        private void SetGridSize()
        {
            var curr = (App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage;

            var content = curr.FindName("ContentPanel");

            if (content != null)
            {
                //Debug.WriteLine("not null");
                //Debug.WriteLine(((content as Grid).ActualWidth / 2) - margin);

                // update gridcell prop which will update view
            }
        }

        private void AddDefaultMenuItems() {
            if (MainMenu != null)
                return;

            IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();

            var menu = new ObservableCollection<MainMenuModel>();

            menu.Add(CreateTile("#1ab154", "conversation", "/Views/Conversation.xaml", "\uf130"));
            menu.Add(CreateTile("#1ec0c3", "weather", "/Views/SingleResult.xaml", "\uf0e9"));
            menu.Add(CreateTile("#f7301e", "notifications", "/Views/Notifications.xaml", "\uf0f3", false, notifications.Count()));
            menu.Add(CreateTile("#bd731b", "notes", "onenote", "\uf15c", true));
            menu.Add(CreateTile("#a3cd53", "search", "/Views/Search.xaml", "\uf002"));
            menu.Add(CreateTile("#9e9e9e", "settings", "/Views/Settings.xaml", "\uf013"));

            MainMenu = menu;
        }

        // TODO: V2 make this whole process dynamic. So any item can be a menu item
        // REFLECTION!!!
        public async void LoadDefaultTemplate(MainMenuModel model)
        {
            var templates = ViewModelLocator.SingleTemplates;

            var singleViewModel = ViewModelLocator.GetServiceInstance<SingleViewModel>();

            Uri page = ViewModelLocator.SingleResultPageUri;

            switch (model.title)
            {
                case "conversation":
                    page = ViewModelLocator.ConversationPageUri;
                    break;

                case "weather":
                    var weather = ViewModelLocator.GetServiceInstance<WeatherViewModel>();
                    weather.GetDefaultForecast();
                    return;
                    break;

                case "notifications":
                    page = ViewModelLocator.NotificationsPageUri;

                    var notifications = ViewModelLocator.GetServiceInstance<NotificationsViewModel>();

                    notifications.LoadNotifications();
                    break;
            
                case "notes":
                    page = ViewModelLocator.NotesUri;                    
                    break;

                case "settings":
                    page = ViewModelLocator.SettingsPageUri;
                    break;

                case "search":
                    page = ViewModelLocator.SearchPageUri;
                    break;

                    /*
                     * default to reflection 
                     */
                    /*
                default:
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
                    break;
                    */
            }

            navigationService.NavigateTo(page);
        }
      
        private MainMenuModel CreateTile(string background, string title, string page, string icon, bool intent = false, object detail = null)
        { 
            var tile = new MainMenuModel()
            {
                background = background,
                title = title,
                page = page,
                icon = icon,
                isIntent = intent
            };

            if (detail != null)
            {
                tile.detail = detail;
                /*
                Type t = detail.GetType();

                if ( t == typeof(string) || (t == typeof(int) && (int)detail > 0) )
                {
                    tile.detail = detail;
                }
                */
            }

            return tile;
        }
    }
}
