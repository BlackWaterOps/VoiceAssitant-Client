using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Phone.Controls;

using GalaSoft.MvvmLight;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlexiVoice.Util;
namespace PlexiVoice.ViewModels
{
    public class ImageViewModel : ViewModelBase
    {
        private IThumbnailedImageAsync initialImage;
        public IThumbnailedImageAsync InitialImage
        {
            get { return initialImage; }
            private set
            {
                initialImage = value;
                RaisePropertyChanged("InitialImage");
            }
        }

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

        // called from ListViewModel to populate collection before navigation
        public void LoadImages(IEnumerable<object> list, string initialImage)
        {
            try
            {
                List<IThumbnailedImageAsync> images = new List<IThumbnailedImageAsync>();

                foreach (string image in list)
                {
                    WebThumbnailedImage thumb = new WebThumbnailedImage(image);
                    images.Add(thumb);

                    if (image == initialImage)
                    {
                        InitialImage = thumb;
                    }
                }

                Images = new ObservableCollection<object>(images);
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("LoadImages Error: {0}", err.Message));
            }
        }

        public Uri GetImageAtIndex(int index)
        {
            object currentImage = Images.ElementAt(index);

            Uri uri = ((WebThumbnailedImage)currentImage).ImagePath;

            return uri;
        }
    }
}
