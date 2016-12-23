using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    public class DetailsViewModel : ViewModelBase
    {
        private object currentItem;
        public object CurrentItem
        {
            get { return currentItem; }
            set
            {
                Debug.WriteLine("set current item");
                currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }

        public DetailsViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
        }
    }
}
