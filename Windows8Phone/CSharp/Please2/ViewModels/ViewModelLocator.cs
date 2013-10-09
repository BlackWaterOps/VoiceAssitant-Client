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
        public const string FullImageUri = @"/Views/FullImage.xaml?image={0}";

        public static readonly Uri MainMenuPageUri = new Uri("/Views/MainPage2.xaml", UriKind.Relative);

        public static readonly Uri ConversationPageUri = new Uri("/Views/Conversation.xaml", UriKind.Relative);

        public static readonly Uri ListResultsPageUri = new Uri("/Views/ListResults.xaml", UriKind.Relative);

        public static readonly Uri SingleResultPageUri = new Uri("/Views/SingleResult.xaml", UriKind.Relative);

        public static readonly Uri NotificationsPageUri = new Uri("/Views/Notifications.xaml", UriKind.Relative);

        public static readonly Uri TimePageUri = new Uri("/Views/Clock.xaml", UriKind.Relative);
        
        public static readonly Uri AlarmPageUri = new Uri("/Views/Alarm.xaml", UriKind.Relative);

        public static readonly Uri ReminderPageUri = new Uri("/Views/Reminder.xaml", UriKind.Relative);

        public static readonly Uri SettingsPageUri = new Uri("/Views/Settings.xaml", UriKind.Relative);

        public static readonly Uri SearchPageUri = new Uri("/Views/Search.xaml", UriKind.Relative);

        public static readonly Uri ImagesPageUri = new Uri("/Views/Images.xaml", UriKind.Relative);


        // When we're all said and done, we should only need the Uris above
        /*
        public static readonly Uri FitbitResultsPageUri = new Uri("/Views/Fitbit.xaml", UriKind.Relative);

        public static readonly Uri WeatherPageUri = new Uri("/Views/Weather.xaml", UriKind.Relative);

        public static readonly Uri StockPageUri = new Uri("/Views/Stock.xaml", UriKind.Relative);

        public static readonly Uri GeoPoliticsPageUri = new Uri("/Views/GeoPolitics.xaml", UriKind.Relative);
        */

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
            get { return GetViewModelInstance<MainMenuViewModel>(); }
        }

        public ConversationViewModel ConversationViewModel
        {
            get { return GetViewModelInstance<ConversationViewModel>(); }
        }

        public ListViewModel ListViewModel
        {
            get { return GetViewModelInstance<ListViewModel>(); }
        }

        public SingleViewModel SingleViewModel
        {
            get { return GetViewModelInstance<SingleViewModel>(); }
        }

        public WeatherViewModel WeatherViewModel
        {
            get { return GetViewModelInstance<WeatherViewModel>(); }
        }

        public StockViewModel StockViewModel
        {
            get { return GetViewModelInstance<StockViewModel>(); }
        }

        public NotificationsViewModel NotificationsViewModel
        {
            get { return GetViewModelInstance<NotificationsViewModel>(); }
        }

        public FitbitViewModel FitbitViewModel
        {
            get { return GetViewModelInstance<FitbitViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        public static T GetViewModelInstance<T>() where T : class
        {
            if (!SimpleIoc.Default.IsRegistered<T>())
            {
                SimpleIoc.Default.Register<T>();
            }

            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}