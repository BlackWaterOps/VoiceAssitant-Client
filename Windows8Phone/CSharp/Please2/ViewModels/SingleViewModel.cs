using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Ioc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

using Plexi;
using Plexi.Util;

//TODO: major refactor
namespace Please2.ViewModels
{
    public class SingleViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
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

        private Visibility? titleVisibility;
        public Visibility TitleVisibility
        {
            get { return (titleVisibility.HasValue == false) ? Visibility.Visible : titleVisibility.Value; }
            set
            {
                titleVisibility = value;
                RaisePropertyChanged("TitleVisibility");
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

        private DataTemplate contentTemplate;
        public DataTemplate ContentTemplate
        {
            get { return contentTemplate; }
            set
            {
                contentTemplate = value;
                RaisePropertyChanged("ContentTemplate");
            }
        }

        private Thickness margin;
        public Thickness Margin
        {
            get { return margin; }
            set
            {
                margin = value;
                RaisePropertyChanged("Margin");
            }
        }

        INavigationService navigationService;

        public SingleViewModel(INavigationService navigationService, IPlexiService plexiService)
        {
            this.navigationService = navigationService;
        }

        public void Reset()
        {
            title = null;
            subTitle = null;
            contentTemplate = null;
        }

        public void RunTest(string templateName)
        {
            try
            {
                var templates = ViewModelLocator.SingleTemplates;

                if (templates[templateName] == null)
                {
                    Debug.WriteLine("could not find template " + templateName);
                    GoHome();
                    return;
                }

                ContentTemplate = templates[templateName] as DataTemplate;

                var singleTest = new Please2.Tests.Single();

                MethodInfo test = singleTest.GetType().GetMethod(templateName.CamelCase() + "Test", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                Debug.WriteLine(templateName.CamelCase());

                if (test == null)
                {
                    Debug.WriteLine("no test found for " + templateName);
                    GoHome();
                    return;
                }

                var response = (Dictionary<string, object>)test.Invoke(singleTest, null);

                if (response.ContainsKey("title"))
                {
                    Title = (string)response["title"];
                }

                if (response.ContainsKey("subtitle"))
                {
                    SubTitle = (string)response["subtitle"];
                }

                if (response.ContainsKey("titlevisibility"))
                {
                    TitleVisibility = (Visibility)response["titlevisibility"];
                }

                if (response.ContainsKey("scheme"))
                {
                    Scheme = (ColorScheme)response["scheme"];
                }

                if (response.ContainsKey("margin"))
                {
                    Margin = (Thickness)response["margin"];
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("run test failure");
                Debug.WriteLine(err.Message);
            }
        }

        private void GoHome()
        {
            navigationService.NavigateTo(ViewModelLocator.MainMenuPageUri);
        }
    }
}
