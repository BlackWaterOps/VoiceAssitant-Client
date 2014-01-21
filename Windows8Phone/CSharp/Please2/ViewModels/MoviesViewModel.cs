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

using PlexiSDK;
namespace Please2.ViewModels
{
    public class MoviesViewModel : ListViewModel
    {
        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }  

        public MoviesViewModel()
        {
            // attach event handlers
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
        }

        public Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<MoviesModel>>();

            this.Scheme = ColorScheme.Commerce;
            this.Title = "movies";

            return new Dictionary<string, object>()
            {
                { "scheme", this.Scheme },
                { "title", this.Title }
            };
        }

        public void MovieItemSelected(MoviesModel movie)
        {
            /*
            MoviesDetailsViewModel vm = ViewModelLocator.GetServiceInstance<MoviesDetailsViewModel>();

            vm.CurrentItem = movie;

            navigationService.NavigateTo(ViewModelLocator.MoviesDetailsUri);
           */
        }
    }
}
