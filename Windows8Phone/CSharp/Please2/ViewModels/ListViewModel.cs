using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class ListViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string pageTitle;

        public string PageTitle
        {
            get { return pageTitle; }
            set
            {
                pageTitle = value;
                RaisePropertyChanged("PageTitle");
            }
        }

        private DataTemplate template;

        public DataTemplate Template
        {
            get { return template; }
            set
            {
                template = value;
                RaisePropertyChanged("Template");
            }
        }

        private IEnumerable<object> listResults;

        public IEnumerable<object> ListResults
        {
            get { return listResults; }
            set
            {
                listResults = value;
                RaisePropertyChanged("ListResults");
            }
        }

        INavigationService navigationService;

        public RelayCommand<EventModel> EventItemSelection { get; set; }
        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }
        public RelayCommand<ShoppingModel> ShoppingItemSelection { get; set; }
        public RelayCommand<NewsModel> NewsItemSelection { get; set; }
        public RelayCommand<RealEstateModel> RealEstateItemSelection { get; set; }
        public RelayCommand<string> ImageItemSelection { get; set; }

        public ListViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;

            AttachEventHandlers();

            FlightTest();
        }

        private void AttachEventHandlers()
        {
            EventItemSelection = new RelayCommand<EventModel>(EventItemSelected);
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
            ShoppingItemSelection = new RelayCommand<ShoppingModel>(ShoppingItemSelected);
            ImageItemSelection = new RelayCommand<string>(ImageItemSelected);
        }

        #region event handlers
        public void EventItemSelected(EventModel e)
        {
            // navigationService.NavigateTo(new Uri("/Views/EventDetailsPage.xaml?id=" + e.id, UriKind.Relative));
            navigationService.NavigateTo(new Uri("/Views/Details.xaml?template=event&id=" + e.id, UriKind.Relative));
        }

        public void MovieItemSelected(MoviesModel movie)
        {
            // navigate to generic details page with movies id and template name
            navigationService.NavigateTo(new Uri("/Views/Details.xaml?template=movie&id=" + movie.id, UriKind.Relative));
        }

        public void ShoppingItemSelected(ShoppingModel product)
        {
            // navigate to generic details page with movies id and template name
            navigationService.NavigateTo(new Uri(product.url, UriKind.Absolute));
        }

        public void ImageItemSelected(string imageUrl)
        {
            navigationService.NavigateTo(new Uri(String.Format(ViewModelLocator.FullImageUri, imageUrl, UriKind.Relative)));
        }
        #endregion

        #region tests
        private void FlightTest()
        {
            try
            {
                var data = "{\"show\":{\"simple\":{\"text\":\"I found multiple flights. Here is the closest match:\n\nDAL116 arrived at 08:09 pm in Atlanta, GA\"},\"structured\":{\"items\":[{\"origin\":{\"city\":\"Atlanta, GA\",\"airport_code\":\"KATL\",\"airport_name\":\"Hartsfield-Jackson Intl\"},\"status\":\"departed\",\"schedule\":{\"estimated_arrival\":\"2013-10-02T06:43:01\",\"actual_departure\":\"2013-10-01T22:13:00\",\"filed_departure\":\"2013-10-01T21:44:00\"},\"destination\":{\"city\":\"Stuttgart\",\"airport_code\":\"EDDS\",\"airport_name\":\"Stuttgart Echterdingen\"},\"delay\":null,\"identification\":\"DAL116\"},{\"origin\":{\"city\":\"Birmingham, AL\",\"airport_code\":\"KBHM\",\"airport_name\":\"Birmingham-Shuttlesworth Intl\"},\"status\":\"arrived\",\"schedule\":{\"estimated_arrival\":\"2013-10-01T20:09:00\",\"actual_departure\":\"2013-10-01T19:37:00\",\"filed_departure\":\"2013-10-01T19:30:00\",\"actual_arrival\":\"2013-10-01T20:09:00\"},\"destination\":{\"city\":\"Atlanta, GA\",\"airport_code\":\"KATL\",\"airport_name\":\"Hartsfield-Jackson Intl\"},\"delay\":null,\"identification\":\"DAL116\"}],\"flight_number\":\"116\",\"airline\":{\"code\":\"DAL\",\"name\":\"Delta Air Lines, Inc.\",\"url\":\"http://www.delta.com/\",\"country\":\"US\",\"phone\":\"+1-800-221-1212\",\"callsign\":\"Delta\",\"location\":\"\",\"shortname\":\"Delta\"},\"template\":\"list:flights\"}},\"speak\":\"I found multiple flights. Here is the closest match:\n\nDAL116 arrived at 08:09 pm in Atlanta, GA\"}";

                var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<Please2.Models.ActorModel>(data);

                var show = actor.show;

                ListResults = ((Newtonsoft.Json.Linq.JToken)show.structured["items"]).ToObject<IEnumerable<Please2.Models.FlightItem>>();

                var templates = App.Current.Resources["TemplateDictionary"] as ResourceDictionary;

                if (templates["flights"] == null)
                {
                    Console.WriteLine("template not found in TemplateDictionary");
                }

                Template = templates["flights"] as DataTemplate;

                pageTitle = "flights";
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private void EventTest()
        {
           
        }

        private void MovieTest()
        {

        }

        private void ShoppingTest()
        {

        }
        #endregion
    }
}
