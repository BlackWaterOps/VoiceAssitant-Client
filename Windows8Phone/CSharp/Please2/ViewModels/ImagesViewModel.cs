using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;
namespace Please2.ViewModels
{
    class ImagesViewModel : ListViewModel
    {
         public RelayCommand<string> ImagesItemSelection { get; set; }

        public ImagesViewModel()
        {
            ImagesItemSelection = new RelayCommand<string>(ImagesItemSelected);
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<string>>();

            this.Scheme = ColorScheme.Default;
            this.Title = "images";

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void ImagesItemSelected(string imageUrl)
        {
            Debug.WriteLine(imageUrl);

            string uri = String.Format(ViewModelLocator.FullImageUri, imageUrl);

            ImageViewModel vm = ViewModelLocator.GetServiceInstance<ImageViewModel>();

            vm.LoadImages(Items, imageUrl);

            //navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
            navigationService.NavigateTo(ViewModelLocator.ImagePageUri);
        }
    }
}
