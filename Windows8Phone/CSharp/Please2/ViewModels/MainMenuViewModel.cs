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

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

using PlexiSDK;
namespace Please2.ViewModels
{
    public class MainMenuViewModel : GalaSoft.MvvmLight.ViewModelBase
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

        private void AddDefaultMenuItems() {
            if (MainMenu != null)
                return;

            //IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();

            AddMenuItem("#1ab154", "conversation", "\uf130", ViewModelLocator.ConversationPageUri);
            AddMenuItem("#1ec0c3", "weather", "\uf0e9", ViewModelLocator.SingleResultPageUri);
            AddMenuItem("#f7301e", "notifications", "\uf0f3", ViewModelLocator.NotificationsPageUri);
            AddMenuItem("#bd731b", "notes", "\uf15c", ViewModelLocator.NotesUri);
            AddMenuItem("#a3cd53", "search", "\uf002", ViewModelLocator.SearchPageUri);
            AddMenuItem("#9e9e9e", "settings", "\uf013", ViewModelLocator.SettingsPageUri);
        }

        public void AddMenuItem(string background, string title, string icon)
        {
            AddMenuItem(background, title, icon, null, true, null);
        }

        public void AddMenuItem(string background, string title, string icon, bool isEnabled)
        {
            AddMenuItem(background, title, icon, null, isEnabled, null);
        }

        public void AddMenuItem(string background, string title, string icon, Uri page)
        {
            AddMenuItem(background, title, icon, page, true, null);
        }

        public void AddMenuItem(string background, string title, string icon, Uri page, bool isEnabled)
        {
            AddMenuItem(background, title, icon, page, isEnabled, null);
        }

        public void AddMenuItem(string background, string title, string icon, Uri page, string details)
        {
            AddMenuItem(background, title, icon, page, true, details);
        }

        public void AddMenuItem(string background, string title, string icon, Uri page, bool isEnabled, string details)
        {
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }

                /*
                DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();

                if (dbUpdater.DatabaseSchemaVersion < 2)
                {
                   
                    dbUpdater.AddTable<Please2.Models.MenuItem>();

                    dbUpdater.Execute();
                }
                */
                Please2.Models.MenuItem menuItem = new Please2.Models.MenuItem()
                {
                    Background = background,
                    Title = title,
                    Icon = icon,
                    Page = page.OriginalString,
                    Enabled = isEnabled,
                    Details = details
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
                // do some reflection to load some default values ie. weather LoadDefault()


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
