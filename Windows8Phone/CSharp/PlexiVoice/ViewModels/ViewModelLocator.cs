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
using System.Diagnostics;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using PlexiVoice.Util;

using PlexiSDK;
namespace PlexiVoice.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public const string FullImageUri = @"/Views/Image.xaml?imageIndex={0}";

        public const string DetailsUri = @"/Views/Details.xaml?template={0}";

        public const string ChildBrowserUri = @"/ChildBrowser.xaml?url={0}&provider={1}";

        public const string NotificationsUri = @"/Views/Notifications.xaml?{0}={1}";

        public static readonly Uri MainPageUri = new Uri("/MainPage.xaml", UriKind.Relative);

        public static readonly Uri DetailsPageUri = new Uri("/Views/Details.xaml", UriKind.Relative);

        public static readonly Uri NotificationsPageUri = new Uri("/Views/Notifications.xaml", UriKind.Relative);

        public static readonly Uri TimePageUri = new Uri("/Views/Clock.xaml", UriKind.Relative);
        
        public static readonly Uri AlarmPageUri = new Uri("/Views/Alarm.xaml", UriKind.Relative);

        public static readonly Uri ReminderPageUri = new Uri("/Views/Reminder.xaml", UriKind.Relative);

        public static readonly Uri SettingsPageUri = new Uri("/Views/Settings.xaml", UriKind.Relative);

        public static readonly Uri SearchPageUri = new Uri("/Views/Search.xaml", UriKind.Relative);

        public static readonly Uri ImagePageUri = new Uri("/Views/Image.xaml", UriKind.Relative);

        public static readonly Uri RegistrationUri = new Uri("/Views/Registration.xaml", UriKind.Relative);

        public static readonly Uri MoviesDetailsUri = new Uri("/Views/MoviesDetails.xaml", UriKind.Relative);

        public static readonly Uri RealEstateDetailsUri = new Uri("/Views/RealEstateDetails.xaml", UriKind.Relative);

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
            
            SimpleIoc.Default.Register<IPlexiService, PlexiSDK.Core>();
            
            SimpleIoc.Default.Register<ISpeechService, SpeechService>();

            SimpleIoc.Default.Register<MainViewModel>(true);
        }

        public MainViewModel MainViewModel
        {
            get { return GetServiceInstance<MainViewModel>(); }
        }

        public DetailsViewModel DetailsViewModel
        {
            get { return GetServiceInstance<DetailsViewModel>(); }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return GetServiceInstance<SettingsViewModel>(); }
        }

        public AlarmDetailsViewModel AlarmDetailsViewModel
        {
            get { return GetServiceInstance<AlarmDetailsViewModel>(); }
        }

        public ReminderDetailsViewModel ReminderDetailsViewModel
        {
            get { return GetServiceInstance<ReminderDetailsViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
            SimpleIoc.Default.Reset();
        }

        public static TInterface GetServiceInstance<TInterface, TClass>() where TInterface : class where TClass : class
        {
            if (!SimpleIoc.Default.IsRegistered<TInterface>())
            {
                SimpleIoc.Default.Register<TInterface, TClass>();
            }

            return ServiceLocator.Current.GetInstance<TInterface>();
        }

        public static T GetServiceInstance<T>() where T : class
        {
            try
            {
                if (!SimpleIoc.Default.IsRegistered<T>())
                {
                    SimpleIoc.Default.Register<T>();
                }

                return ServiceLocator.Current.GetInstance<T>();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return default(T);
            }
        }
    }
}