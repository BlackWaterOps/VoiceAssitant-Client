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

using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private DatabaseModel db = new DatabaseModel(AppResources.DataStore);

        #region Properties
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

        public DateTime SubTitle { get { return DateTime.Now; } }

        private ObservableCollection<Please2.Models.MenuItem> mainMenu;
        public ObservableCollection<Please2.Models.MenuItem> MainMenu
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
        #endregion

        public RelayCommand MenuLoaded { get; set; }

        private INavigationService navigationService;

        public MainMenuViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;

            MenuLoaded = new RelayCommand(SetGridSize);

            InitializeMenu();
        }

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

        private void InitializeMenu()
        {
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }

                IQueryable<Please2.Models.MenuItem> query = from menuItem in db.Menu select menuItem;

                if (query.Count() == 0)
                {
                    AddDefaultMenuItems();
                }

                MainMenu = new ObservableCollection<Models.MenuItem>(query);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void AddDefaultMenuItems() {
            if (MainMenu != null)
                return;

            //IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();

            AddMenuItem(1, "#1ab154", "conversation", "\uf130", ViewModelLocator.ConversationPageUri);
            AddMenuItem(2, "#1ec0c3", "weather", "\uf0e9", ViewModelLocator.SingleResultPageUri, typeof(WeatherViewModel));
            AddMenuItem(3, "#f7301e", "notifications", "\uf0f3", ViewModelLocator.NotificationsPageUri);
            AddMenuItem(4, "#bd731b", "notes", "\uf15c", ViewModelLocator.NotesUri);
            AddMenuItem(5, "#a3cd53", "search", "\uf002", ViewModelLocator.SearchPageUri);
            AddMenuItem(6, "#9e9e9e", "settings", "\uf013", ViewModelLocator.SettingsPageUri);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon)
        {
            AddMenuItem(orderID, background, title, icon, null, null, true, null);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon, bool isEnabled)
        {
            AddMenuItem(orderID, background, title, icon, null, null, isEnabled, null);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon, Uri page)
        {
            AddMenuItem(orderID, background, title, icon, page, null, true, null);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon, Uri page, Type viewModel)
        {
            AddMenuItem(orderID, background, title, icon, page, viewModel, true, null);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon, Uri page, Type viewModel, bool isEnabled)
        {
            AddMenuItem(orderID, background, title, icon, page, viewModel, isEnabled, null);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon, Uri page, Type viewModel, string details)
        {
            AddMenuItem(orderID, background, title, icon, page, viewModel, true, details);
        }

        public void AddMenuItem(int orderID, string background, string title, string icon, Uri page, Type viewModel, bool isEnabled, string details)
        {
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }

                Please2.Models.MenuItem menuItem = new Please2.Models.MenuItem()
                {
                    OrderID = orderID,
                    Background = background,
                    Title = title,
                    Icon = icon,
                    Page = page.OriginalString,
                    Enabled = isEnabled,
                    Details = details,
                    ViewModel = (viewModel != null) ? viewModel.ToString() : null
                };

                db.Menu.InsertOnSubmit(menuItem);

                db.SubmitChanges();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        public void HandleMenuItem(Please2.Models.MenuItem item)
        {
            Uri uri = new Uri(item.Page, UriKind.Relative);

            if (uri == ViewModelLocator.SingleResultPageUri || uri == ViewModelLocator.ListResultsPageUri)
            {
                Type type = Type.GetType(item.ViewModel);

                object vm = Activator.CreateInstance(type);

                MethodInfo loadMethod = vm.GetType().GetMethod("LoadDefault");

                if (loadMethod == null)
                {
                    Debug.WriteLine(String.Format("HandleMenuItem: 'LoadDefault' method not implemented in {0}", item.ViewModel));
                    return;
                }

                // kick off default query for this tile
                loadMethod.Invoke(vm, null);
                return;
            }

            navigationService.NavigateTo(uri);
        }

        /*
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
            }

            navigationService.NavigateTo(page);
        }
        */
        /*
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
            }

            return tile;
        }
         */
    }
}
