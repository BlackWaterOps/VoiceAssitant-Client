using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;

using Please2.Models;
using Please2.Resources;
using Please2.Util;

namespace Please2.ViewModels
{
    public class MoviesViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private List<MoviesModel> upcoming;

        public List<MoviesModel> Upcoming
        {
            get { return upcoming; }
        }

        private List<MoviesModel> nowPlaying;

        public List<MoviesModel> NowPlaying
        {
            get { return nowPlaying; }
        }

        INavigationService navigationService;

        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }  

        public MoviesViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;

            // attach event handlers
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
        }

        public void MovieItemSelected(MoviesModel movie)
        {
            
            // navigate to generic details page with movies id and template name
            navigationService.NavigateTo(new Uri("/Views/Details.xaml?template=movie&id=" + movie.id, UriKind.Relative));
        }
    }
}
