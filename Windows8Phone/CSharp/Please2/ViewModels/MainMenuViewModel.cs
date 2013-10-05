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

using Please2.Models;

namespace Please2.ViewModels
{
    public class MainMenuViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private int columns = 2;

        public string PageTitle { get { return "main menu";  } }

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

        public MainMenuViewModel()
        {
            AddDefaultMenuItems();
        }

        public void AddDefaultMenuItems() {
            if (MainMenu != null)
                return;

            var menu = new ObservableCollection<MainMenuModel>();

            menu.Add(CreateTile("#1ab154", "conversation", "/Views/Conversation.xaml", "\uf130"));
            menu.Add(CreateTile("#1ec0c3", "weather", "/Views/Weather.xaml", "\uf0e9"));
            menu.Add(CreateTile("#f7301e", "notifications", "/Views/Notifications.xaml", "\uf0f3"));
            menu.Add(CreateTile("#bd731b", "notes", "/Views/Notes.xaml", "\uf15c"));
            menu.Add(CreateTile("#a3cd53", "search", "/Views/Search.xaml", "\uf002"));
            menu.Add(CreateTile("#9e9e9e", "settings", "/Views/Settings.xaml", "\uf013"));

            MainMenu = menu;
        }

        private MainMenuModel CreateTile(string background, string title, string page, string icon)
        { 
            var tile = new MainMenuModel()
            {
                background = background,
                title = title,
                page = page,
                icon = icon
            };

            return tile;
        }
    }
}
