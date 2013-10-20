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
using System.Windows;

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
        public static ResourceDictionary ListTemplates = App.Current.Resources["ListTemplateDictionary"] as ResourceDictionary;

        public static ResourceDictionary SingleTemplates = App.Current.Resources["SingleTemplateDictionary"] as ResourceDictionary;

        public static ResourceDictionary DetailsTemplates = App.Current.Resources["DetailsTemplateDictionary"] as ResourceDictionary;

        public const string FullImageUri = @"/Views/Image.xaml?image={0}";

        public const string DetailsUri = @"/Views/Details.xaml?template={0}";

        public static readonly Uri MainMenuPageUri = new Uri("/MainPage2.xaml", UriKind.Relative);

        public static readonly Uri ConversationPageUri = new Uri("/Views/Conversation.xaml", UriKind.Relative);

        public static readonly Uri ListResultsPageUri = new Uri("/Views/List.xaml", UriKind.Relative);

        public static readonly Uri SingleResultPageUri = new Uri("/Views/Single.xaml", UriKind.Relative);

        public static readonly Uri NotificationsPageUri = new Uri("/Views/Notifications.xaml", UriKind.Relative);

        public static readonly Uri TimePageUri = new Uri("/Views/Clock.xaml", UriKind.Relative);
        
        public static readonly Uri AlarmPageUri = new Uri("/Views/Alarm.xaml", UriKind.Relative);

        public static readonly Uri ReminderPageUri = new Uri("/Views/Reminder.xaml", UriKind.Relative);

        public static readonly Uri SettingsPageUri = new Uri("/Views/Settings.xaml", UriKind.Relative);

        public static readonly Uri SearchPageUri = new Uri("/Views/Search.xaml", UriKind.Relative);

        public static readonly Uri ImagePageUri = new Uri("/Views/Image.xaml", UriKind.Relative);

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

        public DetailsViewModel DetailsViewModel
        {
            get { return GetViewModelInstance<DetailsViewModel>(); }
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

        public NewsViewModel NewsViewModel
        {
            get { return GetViewModelInstance<NewsViewModel>(); }
        }

        public ImageViewModel ImageViewModel
        {
            get { return GetViewModelInstance<ImageViewModel>(); }
        }

        public FlightsViewModel FlightsViewModel
        {
            get { return GetViewModelInstance<FlightsViewModel>(); }
        }

        public HoroscopeViewModel HoroscopeViewModel
        {
            get { return GetViewModelInstance<HoroscopeViewModel>(); }
        }

        public GeopoliticsViewModel GeopoliticsViewModel
        {
            get { return GetViewModelInstance<GeopoliticsViewModel>(); }
        }

        public RealEstateViewModel RealEstateViewModel
        {
            get { return GetViewModelInstance<RealEstateViewModel>(); }
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