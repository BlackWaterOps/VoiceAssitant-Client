using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Phone.Controls;

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
        private string templateName;

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

        INavigationService navigationService;
        IPleaseService pleaseService;

        public RelayCommand<EventModel> EventItemSelection { get; set; }
        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }
        public RelayCommand<ShoppingModel> ShoppingItemSelection { get; set; }
        public RelayCommand<NewsModel> NewsItemSelection { get; set; }
        public RelayCommand<RealEstateModel> RealEstateItemSelection { get; set; }
        public RelayCommand<string> ImageItemSelection { get; set; }
        public RelayCommand<AltFuelModel> FuelItemSelection { get; set; }
        public RelayCommand<ChoiceModel> ChoiceItemSelection { get; set; }
        
        public ListViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;
            this.pleaseService = pleaseService;

            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            EventItemSelection = new RelayCommand<EventModel>(EventItemSelected);
            MovieItemSelection = new RelayCommand<MoviesModel>(MovieItemSelected);
            ShoppingItemSelection = new RelayCommand<ShoppingModel>(ShoppingItemSelected);
            ImageItemSelection = new RelayCommand<string>(ImageItemSelected);
            FuelItemSelection = new RelayCommand<AltFuelModel>(FuelItemSelected);
            ChoiceItemSelection = new RelayCommand<ChoiceModel>(ChoiceItemSelected);
        }

        #region event handlers
        public async void ChoiceItemSelected(ChoiceModel choice)
        {
            // pass selection to please service to process and send to auditor
            await pleaseService.Auditor(choice);   
        }

        public void EventItemSelected(EventModel e)
        {
            //navigationService.NavigateTo(new Uri("/Views/EventDetailsPage.xaml?id=" + e.id, UriKind.Relative));

            var isSet = SetDetails(this.templateName, e);

            var uri = String.Format(ViewModelLocator.DetailsUri, this.templateName);

            navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
        }

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
            var uri = String.Format(ViewModelLocator.FullImageUri, imageUrl);

            //var vm = ViewModelLocator.GetViewModelInstance<ImageViewModel>();

            //vm.CurrentImage = imageUrl;

            navigationService.NavigateTo(new Uri(uri, UriKind.Relative));
        }

        public void FuelItemSelected(AltFuelModel fuel)
        {
            //var uri = String.Format(ViewModelLocator.DetailsUri, "fuel", fuel.id);

            // navigate to generic details page with movies id and template name
            //navigationService.NavigateTo(new Uri(uri, UriKind.Absolute));
        }
        #endregion

        #region helpers
        public Dictionary<string, object> Populate(Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            var templates = ViewModelLocator.ListTemplates;

            string[] template = (structured["template"] as string).Split(':');

            this.templateName = template[1];

            if (structured.ContainsKey("items"))
            {
                title = this.templateName + " results";

                if (templates[this.templateName] == null)
                {
                    Debug.WriteLine("template " + this.templateName + " not found in TemplateDictionary");
                    return null;
                }

                // set list to grid view.
                if (this.templateName == "images")
                {
                    LayoutMode = LongListSelectorLayoutMode.Grid;
                    GridCellSize = new Size(145, 145);
                }

                Template = templates[this.templateName] as DataTemplate;

                ListResults = CreateTypedList(this.templateName, structured["items"]);
            }

            // nothing really to send back. everything is set on this page
            return ret;
        }

        public bool SetDetails(string template, object model)
        {
            var templates = ViewModelLocator.DetailsTemplates;

            var isSet = false;

            if (templates[template] != null)
            {
                var vm = ViewModelLocator.GetViewModelInstance<DetailsViewModel>();

                vm.CurrentItem = model;
                isSet = true;
            }

            return isSet;
        }

        public IEnumerable<object> CreateTypedList(string name, object items)
        {
            IEnumerable<object> ret = new List<object>();

            var arr = items as JArray;

            switch (name)
            {
                case "images":
                    ret = arr.ToObject<IEnumerable<string>>();
                    break;

                case "fuel":
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

                case "choice":
                    ret = arr.ToObject<IEnumerable<ChoiceModel>>();
                    break;
            }

            return ret;
        }

        public void RunTest(string templateName)
        {
            try
            {
                var templates = ViewModelLocator.ListTemplates;

                if (templates[templateName] == null)
                {
                    Debug.WriteLine("could not find template " + templateName);
                    return;
                }

                this.templateName = templateName;
                
                Template = templates[templateName] as DataTemplate;

                var listTest = new Please2.Tests.List();

                var test = listTest.GetType().GetMethod((Char.ToUpper(templateName[0]) + templateName.Substring(1)) + "Test", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
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

                if (response.ContainsKey("layoutmode"))
                {
                    LayoutMode = (LongListSelectorLayoutMode)response["layoutmode"];
                }

                if (response.ContainsKey("gridcellsize"))
                {
                    GridCellSize = (Size)response["gridcellsize"];
                }

                Debug.WriteLine("bottom");
            }
            catch (Exception err)
            {
                Debug.WriteLine("outer excep: " + err.Message);
            }
        }
        #endregion
    }
}
