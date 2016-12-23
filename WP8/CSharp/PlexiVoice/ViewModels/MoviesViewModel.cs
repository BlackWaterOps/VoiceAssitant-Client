using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    public class MoviesViewModel : ListBase
    {
        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }  

        public MoviesViewModel()
        {
            // attach event handlers
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
        }

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            Items = (structured["items"] as JArray).ToObject<List<MoviesModel>>();
        }

        public void MovieItemSelected(MoviesModel movie)
        {
            /*
            MovieDetailsViewModel vm = ViewModelLocator.GetServiceInstance<MovieDetailsViewModel>();

            vm.CurrentItem = movie;

            navigationService.NavigateTo(ViewModelLocator.MoviesDetailsUri);
            */

            MovieDetailsViewModel details = new MovieDetailsViewModel();

            details.CurrentItem = movie;

            DetailsViewModel vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

            vm.CurrentItem = details;

            navigationService.NavigateTo(ViewModelLocator.DetailsPageUri);
        }
    }
}
