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

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
using PlexiSDK.Models;
using PlexiSDK.Util;
namespace PlexiVoice.ViewModels
{
    public class ListBase : ViewModelBase
    {        
        private IEnumerable<object> items;
        public IEnumerable<object> Items
        {
            get { return items; }
            set
            {
                items = value;
                RaisePropertyChanged("Items");
            }
        }

        protected INavigationService navigationService;
        protected IPlexiService plexiService;

        public RelayCommand<SelectionChangedEventArgs> ListItemSelection { get; set; }  

        public ListBase()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            ListItemSelection = new RelayCommand<SelectionChangedEventArgs>(ListItemSelected);
        }

        public void ListItemSelected(SelectionChangedEventArgs e)
        {
            Debug.WriteLine("list item selected");

            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e));
        }
    }
}
