using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;
namespace PlexiVoice.ViewModels
{
    class ImagesViewModel : ListBase
    {
         public RelayCommand<string> ImagesItemSelection { get; set; }

        public ImagesViewModel()
        {
            ImagesItemSelection = new RelayCommand<string>(ImagesItemSelected);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<string>>();
        }

        public void ImagesItemSelected(string imageUrl)
        {
            string uri = String.Format(ViewModelLocator.FullImageUri, imageUrl);

            ImageViewModel vm = ViewModelLocator.GetServiceInstance<ImageViewModel>();

            vm.LoadImages(Items, imageUrl);

            //navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
            navigationService.NavigateTo(ViewModelLocator.ImagePageUri);
        }
    }
}
