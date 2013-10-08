using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class MainMenuViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private int columns = 2;

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
                var w = App.Current.Host.Content.ActualWidth / columns;

                Debug.WriteLine(w);

                return new Size(221, 221);
            }
        }

        private INavigationService navigationService = SimpleIoc.Default.GetInstance<INavigationService>();

        public MainMenuViewModel()
        {
            AddDefaultMenuItems();
        }

        public void AddDefaultMenuItems() {
            if (MainMenu != null)
                return;

            var menu = new ObservableCollection<MainMenuModel>();

            menu.Add(CreateTile("#1ab154", "conversation", "/Views/Conversation.xaml", "\uf130"));
            menu.Add(CreateTile("#1ec0c3", "weather", "/Views/SingleResult.xaml", "\uf0e9"));
            menu.Add(CreateTile("#f7301e", "notifications", "/Views/Notifications.xaml", "\uf0f3"));
            menu.Add(CreateTile("#bd731b", "notes", "onenote:", "\uf15c", true));
            menu.Add(CreateTile("#a3cd53", "search", "/Views/Search.xaml", "\uf002"));
            menu.Add(CreateTile("#9e9e9e", "settings", "/Views/Settings.xaml", "\uf013"));

            MainMenu = menu;
        }

        // TODO: V2 make this whole process dynamic. So any item can be a menu item
        public async void LoadDefaultTemplate(MainMenuModel model)
        {
            var templates = App.Current.Resources["SingleTemplateDictionary"] as ResourceDictionary;

            var singleViewModel = ViewModelLocator.GetViewModelInstance<SingleViewModel>();

            Uri page = ViewModelLocator.SingleResultPageUri;

            switch (model.title)
            {
                case "conversation":
                    page = ViewModelLocator.ConversationPageUri;
                    break;

                case "weather":
                    var weather = App.GetViewModelInstance<WeatherViewModel>();
                    weather.GetDefaultForecast();

                    singleViewModel.Title = "weather";

                    var pos = Please2.Util.Location.GeoPosition;
                    /*
                    if (pos != null)
                    {
                        subtitle += pos.CivicAddress.City + ", " + pos.CivicAddress.State + ": ";
                    }
                    */
                    singleViewModel.SubTitle = DateTime.Now.ToString("dddd, MMMM d, yyyy");
                    singleViewModel.ContentTemplate = templates[model.title] as DataTemplate;
                    break;

                case "notifications":
                    page = ViewModelLocator.NotificationsPageUri;

                    var notifications = App.GetViewModelInstance<NotificationsViewModel>();

                    notifications.LoadNotifications();
                    break;
            
                case "notes":
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("onenote://?todo=mytesttext"));
                    return;
                    break;

                case "settings":
                    page = ViewModelLocator.SettingsPageUri;
                    break;

                case "search":

                    break;
            }

            navigationService.NavigateTo(page);
        }
      
        private MainMenuModel CreateTile(string background, string title, string page, string icon, bool initent = false)
        { 
            var tile = new MainMenuModel()
            {
                background = background,
                title = title,
                page = page,
                icon = icon,
                isIntent = initent
            };

            return tile;
        }
    }
}
