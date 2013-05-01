using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Newtonsoft.Json;

namespace Please.ViewModels
{
    public class GalleryViewModel : ViewModelBase
    {

        // list of spoken and response text
        private ObservableCollection<BitmapImage> _galleryList;
        public ObservableCollection<BitmapImage> GalleryList
        {
            get
            {
                return _galleryList;
            }
            set
            {
                _galleryList = value;
                NotifyPropertyChanged("PleaseList");
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                _searchTerm = value;
                NotifyPropertyChanged("SearchTerm");
            }
        }

        public void LoadImages(Dictionary<string, object> payload)
        {
            if (GalleryList == null)
            {
                GalleryList = new ObservableCollection<BitmapImage>();
            }
            else
            {
                // clear collection for new set of images?
            }

            if (payload.ContainsKey("url"))
            {
                var urls = payload["url"] as Newtonsoft.Json.Linq.JArray;

                foreach (string url in urls)
                {
                    GalleryList.Add( new BitmapImage(new Uri(url, UriKind.Absolute)) );
                }
            }
        }
    }
}
