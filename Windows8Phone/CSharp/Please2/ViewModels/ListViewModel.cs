﻿using System;
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

using Plexi;
using Plexi.Models;
using Plexi.Util;

namespace Please2.ViewModels
{
    public class ListViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string templateName;

        private string scheme;
        public string Scheme
        {
            get { return scheme; }
            set 
            {
                scheme = value.CamelCase();
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
        IPlexiService plexiService;

        public RelayCommand<EventModel> EventItemSelection { get; set; }
        public RelayCommand<MoviesModel> MovieItemSelection { get; set; }
        public RelayCommand<ShoppingModel> ShoppingItemSelection { get; set; }
        public RelayCommand<NewsModel> NewsItemSelection { get; set; }
        public RelayCommand<RealEstateModel> RealEstateItemSelection { get; set; }
        public RelayCommand<string> ImageItemSelection { get; set; }
        public RelayCommand<AltFuelModel> FuelItemSelection { get; set; }
        public RelayCommand<ChoiceModel> ChoiceItemSelection { get; set; }
        public RelayCommand<SearchModel> SearchItemSelection { get; set; }

        public ListViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;
            this.plexiService = plexiService;

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
            SearchItemSelection = new RelayCommand<SearchModel>(SearchItemSelected);
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

            vm.AddDialog("user", choice.text);

            // pass selection to please service to process and send to auditor
            plexiService.Choice(choice);
        }

        public void EventItemSelected(EventModel e)
        {
            //navigationService.NavigateTo(new Uri("/Views/EventDetailsPage.xaml?id=" + e.id, UriKind.Relative));

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
            var isSet = SetDetails(this.templateName, fuel);

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
                    Debug.WriteLine(String.Format("template {0} not found in TemplateDictionary", this.templateName));
                    GoTo("conversation");
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
                GoTo("conversation");
            }

            // nothing really to send back. everything is set on this page
            return null;
        }

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

        public IEnumerable<object> CreateTypedList(string name, object items)
        {
            IEnumerable<object> data = new List<object>();

            var arr = items as JArray;

            switch (name)
            {
                case "images":
                    data = arr.ToObject<IEnumerable<string>>();
                    Scheme = "default";
                    break;

                case "fuel":
                    data = arr.ToObject<IEnumerable<AltFuelModel>>();
                    Scheme = "information";
                    break;

                case "product":
                case "shopping":
                    data = arr.ToObject<IEnumerable<ShoppingModel>>();
                    Scheme = "commerce";
                    break;

                case "events":
                    data = arr.ToObject<IEnumerable<EventModel>>();
                    Scheme = "commerce";
                    break;

                case "movies":
                    data = arr.ToObject<IEnumerable<MoviesModel>>();
                    Scheme = "commerce";
                    break;

                case "choice":
                    data = arr.ToObject<IEnumerable<ChoiceModel>>();
                    Scheme = "default";
                    break;

                case "search":
                    data = arr.ToObject<IEnumerable<SearchModel>>();
                    Scheme = "information";
                    break;
            }

            return data;
        }

        public void RunTest(string templateName)
        {
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
