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

using Please2.Util;

using Plexi;

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

        public const string FullImageUri = @"/Views/Image.xaml?imageIndex={0}";

        public const string DetailsUri = @"/Views/Details.xaml?template={0}";

        public const string ChildBrowserUri = @"/ChildBrowser.xaml?url={0}&isOptional={1}";

        public const string NoteUri = @"/Views/Note.xaml?noteid={0}";

        public static readonly Uri MainMenuPageUri = new Uri("/MainMenu.xaml", UriKind.Relative);

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

        public static readonly Uri RegistrationUri = new Uri("/Views/Registration.xaml", UriKind.Relative);

        public static readonly Uri NotesUri = new Uri("/Views/Notes.xaml", UriKind.Relative);

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
            
            SimpleIoc.Default.Register<IPlexiService, Plexi.Core>();
            
            SimpleIoc.Default.Register<ISpeechService, SpeechService>();

            SimpleIoc.Default.Register<MainMenuViewModel>();
            SimpleIoc.Default.Register<ConversationViewModel>();

            new PlexiHandler();
        }

        public MainMenuViewModel MainMenuViewModel
        {
            get { return GetServiceInstance<MainMenuViewModel>(); }
        }

        public ConversationViewModel ConversationViewModel
        {
            get { return GetServiceInstance<ConversationViewModel>(); }
        }

        public ListViewModel ListViewModel
        {
            get { return GetServiceInstance<ListViewModel>(); }
        }

        public SingleViewModel SingleViewModel
        {
            get { return GetServiceInstance<SingleViewModel>(); }
        }

        public DetailsViewModel DetailsViewModel
        {
            get { return GetServiceInstance<DetailsViewModel>(); }
        }

        public WeatherViewModel WeatherViewModel
        {
            get { return GetServiceInstance<WeatherViewModel>(); }
        }

        public StockViewModel StockViewModel
        {
            get { return GetServiceInstance<StockViewModel>(); }
        }

        public NotificationsViewModel NotificationsViewModel
        {
            get { return GetServiceInstance<NotificationsViewModel>(); }
        }

        public FitbitViewModel FitbitViewModel
        {
            get { return GetServiceInstance<FitbitViewModel>(); }
        }

        public NewsViewModel NewsViewModel
        {
            get { return GetServiceInstance<NewsViewModel>(); }
        }

        public ImageViewModel ImageViewModel
        {
            get { return GetServiceInstance<ImageViewModel>(); }
        }

        public FlightsViewModel FlightsViewModel
        {
            get { return GetServiceInstance<FlightsViewModel>(); }
        }

        public HoroscopeViewModel HoroscopeViewModel
        {
            get { return GetServiceInstance<HoroscopeViewModel>(); }
        }

        public GeopoliticsViewModel GeopoliticsViewModel
        {
            get { return GetServiceInstance<GeopoliticsViewModel>(); }
        }

        public RealEstateViewModel RealEstateViewModel
        {
            get { return GetServiceInstance<RealEstateViewModel>(); }
        }

        public DictionaryViewModel DictionaryViewModel
        {
            get { return GetServiceInstance<DictionaryViewModel>(); }
        }

        public NotesViewModel NotesViewModel
        {
            get { return GetServiceInstance<NotesViewModel>(); }
        }
        /*
        public TimeViewModel TimeViewModel
        {
            get { return GetViewModelInstance<TimeViewModel>(); }
        }
        */
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        public static T GetServiceInstance<T>() where T : class
        {
            if (!SimpleIoc.Default.IsRegistered<T>())
            {
                SimpleIoc.Default.Register<T>();
            }

            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}