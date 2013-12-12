using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Phone.Controls;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Util;

using Plexi;

namespace Please2.ViewModels
{
    public class ImageViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private ObservableCollection<object> images;
        public ObservableCollection<object> Images
        {
            get { return images; }
            set
            {
                images = value;
                RaisePropertyChanged("Images");
            }
        }

        public ImageViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            LoadImagesFromListViewModel();
        }

        private void LoadImagesFromListViewModel()
        {
            try
            {
                ListViewModel vm = ViewModelLocator.GetServiceInstance<ListViewModel>();

                IEnumerable<object> results = vm.ListResults;

                List<IThumbnailedImageAsync> images = new List<IThumbnailedImageAsync>();
                foreach (string image in results)
                {
                    images.Add(new WebThumbnailedImage(image));
                }

                Images = new ObservableCollection<object>(images);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
