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
        public static readonly Uri MainMenuPageUri = new Uri("/Views/Conversation.xaml", UriKind.Relative);

        public static readonly Uri ConversationPageUri = new Uri("/Views/Conversation.xaml", UriKind.Relative);

        public static readonly Uri ListResultsPageUri = new Uri("/Views/ListResults.xaml", UriKind.Relative);

        public static readonly Uri SingleResultPageUri = new Uri("/Views/SingleResult.xaml", UriKind.Relative);



        // When we're all said and done, we should only need the Uris above

        public static readonly Uri FitbitResultsPageUri = new Uri("/Views/Fitbit.xaml", UriKind.Relative);

        public static readonly Uri WeatherPageUri = new Uri("/Views/Weather.xaml", UriKind.Relative);

        public static readonly Uri ImagesPageUri = new Uri("/Views/Images.xaml", UriKind.Relative);

        public static readonly Uri StockPageUri = new Uri("/Views/Stock.xaml", UriKind.Relative);

        public static readonly Uri GeoPoliticsPageUri = new Uri("/Views/GeoPolitics.xaml", UriKind.Relative);

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
        }

        public MainMenuViewModel MainMenuViewModel
        {
            get
            {
                return App.GetViewModelInstance<MainMenuViewModel>();
            }
        }

        public ConversationViewModel ConversationViewModel
        {
            get
            {
                return App.GetViewModelInstance<ConversationViewModel>();
            }
        }

        public WeatherViewModel WeatherViewModel
        {
            get
            {
                return App.GetViewModelInstance<WeatherViewModel>();
            }
        }

        public StockViewModel StockViewModel
        {
            get
            {
                return App.GetViewModelInstance<StockViewModel>();
            }
        }

        public EventsViewModel EventsViewModel
        {
            get
            {
                return App.GetViewModelInstance<EventsViewModel>();
            }
        }

        public NotificationsViewModel NotificationsViewModel
        {
            get
            {
                return App.GetViewModelInstance<NotificationsViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}