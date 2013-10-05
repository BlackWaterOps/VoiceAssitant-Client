/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Please2"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/
using System;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using Please2.Util;

namespace Please2.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static readonly Uri ConverationPageUri = new Uri("/Views/Conversation.xaml", UriKind.Relative);

        public static readonly Uri ListResultsPageUri = new Uri("/Views/ListResults.xaml", UriKind.Relative);

        public static readonly Uri SingleResultPageUri = new Uri("/views/SingleResult.xaml", UriKind.Relative);

        public static readonly Uri FitbitResultsPageUri = new Uri("/views/Fitbit.xaml", UriKind.Relative);

        public static readonly Uri WeatherPageUri = new Uri("/views/Weather.xaml", UriKind.Relative);

        public static readonly Uri ImagesPageUri = new Uri("/views/Images.xaml", UriKind.Relative);
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            
            SimpleIoc.Default.Register<IPleaseService, PleaseService>();

            SimpleIoc.Default.Register<MainMenuViewModel>();
            SimpleIoc.Default.Register<ConversationViewModel>();
            SimpleIoc.Default.Register<WeatherViewModel>();

        }

        public MainMenuViewModel MainMenuViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainMenuViewModel>();
            }
        }

        public ConversationViewModel ConversationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConversationViewModel>();
            }
        }

        public WeatherViewModel WeatherViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WeatherViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}