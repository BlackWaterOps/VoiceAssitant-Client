using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight.Ioc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class SingleViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private const string templateDict = "SingleTemplateDictionary";

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

        public SingleViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {

        }

        public void RunTest(string templateName)
        {
            try
            {
                var templates = App.Current.Resources["SingleTemplateDictionary"] as ResourceDictionary;

                if (templates[templateName] == null)
                {
                    Debug.WriteLine("cuold not find template " + templateName);
                    return;
                }

                ContentTemplate = templates[templateName] as DataTemplate;
                Title = templateName;
                SubTitle = "";

                var singleTest = new Please2.Tests.Single();

                var test = singleTest.GetType().GetMethod((Char.ToUpper(templateName[0]) + templateName.Substring(1)) + "Test", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (test == null)
                {
                    Debug.WriteLine("no test found for " + templateName);
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
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
