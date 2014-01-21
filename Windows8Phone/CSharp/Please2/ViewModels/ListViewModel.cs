using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using PlexiSDK;
using PlexiSDK.Models;
using PlexiSDK.Util;
namespace Please2.ViewModels
{
    public class ListViewModel : ViewModelBase
    {
        private string templateName;
        
        #region properties
        private ColorScheme scheme;
        public ColorScheme Scheme
        {
            get { return scheme; }
            set 
            {
                scheme = value;
                RaisePropertyChanged("Scheme");
            }
        }

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
        /*
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

        private LongListSelectorLayoutMode layoutMode;
        public LongListSelectorLayoutMode LayoutMode
        {
            get { return layoutMode; }
            set
            {
                layoutMode = value;
                RaisePropertyChanged("LayoutMode");
            }
        }

        private Size gridCellSize;
        public Size GridCellSize
        {
            get { return gridCellSize; }
            set
            {
                gridCellSize = value;
                RaisePropertyChanged("GridCellSize");
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
        */
        private IEnumerable<object> items;
        public IEnumerable<object> Items
        {
            get { return items; }
            set
            {
                items = value;
                RaisePropertyChanged("Items");
            }
        }
        #endregion

        protected INavigationService navigationService;
        protected IPlexiService plexiService;
        /*
        public RelayCommand<EventModel> EventItemSelection { get; set; }
        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }
        public RelayCommand<ShoppingModel> ShoppingItemSelection { get; set; }
        public RelayCommand<NewsModel> NewsItemSelection { get; set; }
        public RelayCommand<RealEstateModel> RealEstateItemSelection { get; set; }
        public RelayCommand<string> ImageItemSelection { get; set; }
        public RelayCommand<AltFuelModel> FuelItemSelection { get; set; }
        public RelayCommand<ChoiceModel> ChoiceItemSelection { get; set; }
        public RelayCommand<SearchModel> SearchItemSelection { get; set; }
        */
        public ListViewModel()
        {
            this.navigationService = ViewModelLocator.GetServiceInstance<INavigationService>();
            this.plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();

            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            /*
            EventItemSelection = new RelayCommand<EventModel>(EventItemSelected);
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
            ShoppingItemSelection = new RelayCommand<ShoppingModel>(ShoppingItemSelected);
            ImageItemSelection = new RelayCommand<string>(ImageItemSelected);
            FuelItemSelection = new RelayCommand<AltFuelModel>(FuelItemSelected);
            ChoiceItemSelection = new RelayCommand<ChoiceModel>(ChoiceItemSelected);
            SearchItemSelection = new RelayCommand<SearchModel>(SearchItemSelected);
            */
        }

        #region event handlers
        public void SearchItemSelected(SearchModel result)
        {
            Debug.WriteLine(result.url);
            // pass selection to please service to process and send to auditor
            var browser = new WebBrowserTask();
            browser.Uri = new Uri(result.url, UriKind.Absolute);
            browser.Show();
        }

        public void ChoiceItemSelected(ChoiceModel choice)
        {
            // add choice to conversation page
            ConversationViewModel vm = ViewModelLocator.GetServiceInstance<ConversationViewModel>();

            vm.AddDialog(DialogOwner.User, choice.text);

            // pass selection to please service to process and send to auditor
            plexiService.Choice(choice);
        }

        public void EventItemSelected(EventModel model)
        {
            var vm = ViewModelLocator.GetServiceInstance<EventDetailsViewModel>();

            vm.CurrentItem = model;
            vm.Scheme = this.Scheme;
            vm.Title = model.title;

            navigationService.NavigateTo(new Uri("/Views/EventDetails.xaml", UriKind.Relative));
        }

        /*
        public void EventItemSelected(EventModel e)
        {
            var isSet = SetDetails(this.templateName, e);

            if (isSet)
            {
                var uri = String.Format(ViewModelLocator.DetailsUri, this.templateName);

                navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
            }
            else
            {
                // no template found message
            }
        }
        */

        public void MovieItemSelected(MoviesModel movie)
        {
            // navigate to generic details page with movies id and template name
            //var uri = String.Format(ViewModelLocator.DetailsUri, "movie", movie.id);

            //navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
        }

        public void ShoppingItemSelected(ShoppingModel product)
        {
            // navigate to generic details page with movies id and template name
            //navigationService.NavigateTo(new Uri(product.url, UriKind.Absolute));
        }

        public void ImageItemSelected(string imageUrl)
        {
            /*
            string uri = String.Format(ViewModelLocator.FullImageUri, imageUrl);

            ImageViewModel vm = ViewModelLocator.GetServiceInstance<ImageViewModel>();

            vm.LoadImages(Images, imageUrl);

            //navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
            navigationService.NavigateTo(ViewModelLocator.ImagePageUri);
            */
        }

        public void FuelItemSelected(AltFuelModel model)
        {
            FuelDetailsViewModel vm = ViewModelLocator.GetServiceInstance<FuelDetailsViewModel>();

            vm.CurrentItem = model;
            vm.Title = model.station_name;
            vm.Scheme = this.Scheme;

            navigationService.NavigateTo(new Uri("/Views/FuelDetails.xaml", UriKind.Relative));
        }
        #endregion

        #region helpers
        /*
        public Dictionary<string, object> Load(Dictionary<string, object> structured)
        {
            var templates = ViewModelLocator.ListTemplates;

            string[] template = (structured["template"] as string).Split(':');

            this.templateName = template[1];

            if (structured.ContainsKey("items"))
            {
                title = this.templateName + " results";

                if (templates[this.templateName] == null)
                {
                    Debug.WriteLine(String.Format("ListViewModel: template {0} not found in TemplateDictionary", this.templateName));
                    return null;
                }

                Template = templates[this.templateName] as DataTemplate;

                ListResults = CreateTypedList(this.templateName, structured["items"]);

                LayoutMode = LongListSelectorLayoutMode.List;

                // set list to grid view.
                if (this.templateName == "images")
                {
                    LayoutMode = LongListSelectorLayoutMode.Grid;
                    GridCellSize = new Size(145, 145);
                }
            }
            else
            {
                Debug.WriteLine("could not find \"items\" key in structured response");
                return null;
            }

            // nothing really to send back. everything is set on this page
            return new Dictionary<string, object>();
        }
        */
        /*
        public bool SetDetails(string template, object model)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            var isSet = false;

            if (templates[template] != null)
            {
                var vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

                vm.CurrentItem = model;
                isSet = true;
            }

            return isSet;
        }
        */
        /*
        public IEnumerable<object> CreateTypedList(string name, object items)
        {
            IEnumerable<object> data = new List<object>();

            var arr = items as JArray;

            switch (name)
            {
                case "images":
                    data = arr.ToObject<IEnumerable<string>>();
                    Scheme = ColorScheme.Default;
                    break;

                case "fuel":
                    data = arr.ToObject<IEnumerable<AltFuelModel>>();
                    Scheme = ColorScheme.Information;
                    break;

                case "product":
                case "shopping":
                    data = arr.ToObject<IEnumerable<ShoppingModel>>();
                    Scheme = ColorScheme.Commerce;
                    break;

                case "events":
                    data = arr.ToObject<IEnumerable<EventModel>>();
                    Scheme = ColorScheme.Commerce;
                    break;

                case "movies":
                    data = arr.ToObject<IEnumerable<MoviesModel>>();
                    Scheme = ColorScheme.Commerce;
                    break;

                case "choice":
                    data = arr.ToObject<IEnumerable<ChoiceModel>>();
                    Scheme = ColorScheme.Default;
                    break;

                case "search":
                    data = arr.ToObject<IEnumerable<SearchModel>>();
                    Scheme = ColorScheme.Information;
                    break;
            }

            return data;
        }
        */
        public void RunTest(string templateName)
        {
            /*
            try
            {
                var templates = ViewModelLocator.ListTemplates;

                if (templates[templateName] == null)
                {
                    Debug.WriteLine("could not find template " + templateName);
                    GoTo("home");
                    return;
                }

                this.templateName = templateName;
                
                Template = templates[templateName] as DataTemplate;

                var listTest = new Please2.Tests.List();

                var test = listTest.GetType().GetMethod(templateName.CamelCase() + "Test", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (test == null)
                {
                    Debug.WriteLine("no test found for " + templateName);
                    GoTo("home");
                    return;
                }
                
                var response = (Dictionary<string, object>)test.Invoke(listTest, null);
                
                if (!response.ContainsKey("list"))
                {
                    Debug.WriteLine("no list data was returned from test " + templateName);
                    GoTo("home");
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

                if (response.ContainsKey("layoutmode"))
                {
                    LayoutMode = (LongListSelectorLayoutMode)response["layoutmode"];
                }

                if (response.ContainsKey("gridcellsize"))
                {
                    GridCellSize = (Size)response["gridcellsize"];
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("run test: " + err.Message);
            }
             */
        }

        private void GoTo(string location)
        {
            Uri place = null;

            switch (location)
            {
                case "home":
                case "menu":
                    place = ViewModelLocator.MainMenuPageUri;
                    break;

                case "conversation":
                case "dialog":
                    place = ViewModelLocator.ConversationPageUri;
                    break;
            }

            navigationService.NavigateTo(place);
        }
        #endregion
    }
}
