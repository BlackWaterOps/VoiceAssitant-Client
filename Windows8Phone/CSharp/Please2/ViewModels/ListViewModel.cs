using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class ListViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string subTitle;
        public string SubTitle
        {
            get { return subTitle; }
            set
            {
                subTitle = value;
                RaisePropertyChanged("SubTitle");
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
        public RelayCommand<AltFuelModel> FuelItemSelection { get; set; }

        public ListViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;

            //AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            EventItemSelection = new RelayCommand<EventModel>(EventItemSelected);
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
            ShoppingItemSelection = new RelayCommand<ShoppingModel>(ShoppingItemSelected);
            ImageItemSelection = new RelayCommand<string>(ImageItemSelected);
            FuelItemSelection = new RelayCommand<AltFuelModel>(FuelItemSelected);
        }

        #region event handlers
        public void EventItemSelected(EventModel e)
        {
            // navigationService.NavigateTo(new Uri("/Views/EventDetailsPage.xaml?id=" + e.id, UriKind.Relative));
            var uri = String.Format(ViewModelLocator.DetailsUri, "event", e.id);

            navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
        }

        public void MovieItemSelected(MoviesModel movie)
        {
            // navigate to generic details page with movies id and template name
            var uri = String.Format(ViewModelLocator.DetailsUri, "movie", movie.id);

            navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
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

        public void FuelItemSelected(AltFuelModel fuel)
        {
            var uri = String.Format(ViewModelLocator.DetailsUri, "fuel", fuel.id);

            // navigate to generic details page with movies id and template name
            navigationService.NavigateTo(new Uri(uri, UriKind.Absolute));
        }
        #endregion

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            var templates = App.Current.Resources["ListTemplateDictionary"] as ResourceDictionary;

            if (structured.ContainsKey("items"))
            {
                title = templateName + " results";

                if (templates[templateName] == null)
                {
                    Debug.WriteLine("template not found in TemplateDictionary");
                }

                template = templates[templateName] as DataTemplate;

                listResults = CreateTypedList(templateName, structured["items"]);
            }

            // nothing really to send back. everything is set on this page
            return ret;
        }

        private IEnumerable<object> CreateTypedList(string name, object items)
        {
            Debug.WriteLine("create typed list");
            IEnumerable<object> ret = new List<object>();

            var arr = items as JArray;

            switch (name)
            {
                case "fuel":
                    Debug.WriteLine("fuel case");
                    ret = arr.ToObject<IEnumerable<AltFuelModel>>();
                    break;

                case "product":
                case "shopping":
                    ret = arr.ToObject<IEnumerable<ShoppingModel>>();
                    break;

                case "events":
                    ret = arr.ToObject<IEnumerable<EventModel>>();
                    break;

                case "movies":
                    ret = arr.ToObject<IEnumerable<MoviesModel>>();
                    break;

                case "real_estate":
                    ret = arr.ToObject<IEnumerable<RealEstateModel>>();
                    break;
            }

            return ret;
        }

        public void RunTest(string templateName)
        {
            try
            {
                var templates = App.Current.Resources["ListTemplateDictionary"] as ResourceDictionary;

                if (templates[templateName] == null)
                {
                    Debug.WriteLine("could not find template " + templateName);
                    return;
                }

                Template = templates[templateName] as DataTemplate;
                Title = templateName;
                SubTitle = "";

                var listTest = new Please2.Tests.List();

                var test = listTest.GetType().GetMethod((Char.ToUpper(templateName[0]) + templateName.Substring(1)) + "Test", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (test == null)
                {
                    Debug.WriteLine("no test found for " + templateName);
                    return;
                }
                
                var response = (Dictionary<string, object>)test.Invoke(listTest, null);
                
                if (!response.ContainsKey("list"))
                {
                    Debug.WriteLine("no list data was returned from test " + templateName);
                    return;
                }
                
                ListResults = (IEnumerable<object>)response["list"];

                if (response.ContainsKey("title"))
                {
                    Title = (string)response["title"];
                }

                if (response.ContainsKey("subtitle"))
                {
                    SubTitle = (string)response["subtitle"];
                }    
            }
            catch (Exception err)
            {
                Debug.WriteLine("outer excep: " + err.Message);
            }
        }
    }
}
