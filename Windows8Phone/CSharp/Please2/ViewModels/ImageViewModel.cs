using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Util;

namespace Please2.ViewModels
{
    public class ImageViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string prevImage;
        public string PrevImage
        {
            get { return prevImage; }
            set
            {
                prevImage = value;
                RaisePropertyChanged("PrevImage");
            }
        }

        private string nextImage;
        public string NextImage
        {
            get { return nextImage; }
            set
            {
                nextImage = value;
                RaisePropertyChanged("NextImage");
            }
        }

        private string currentImage;
        public string CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                RaisePropertyChanged("CurrentImage");
            }
        }

        public double ScreenWidth
        {
            get { return App.Current.Host.Content.ActualWidth; }
        }
    }
}
