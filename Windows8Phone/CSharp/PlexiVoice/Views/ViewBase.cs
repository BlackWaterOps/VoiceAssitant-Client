using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage; // remove after beta is over
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

using LinqToVisualTree;

using Newtonsoft.Json;

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using PlexiVoice.Controls;
using PlexiVoice.Models;
using PlexiVoice.Util;
using PlexiVoice.ViewModels;

using PlexiSDK;
namespace PlexiVoice.Views
{
    public class ViewBase : PhoneApplicationPage, IDialogService
    {
        private IPlexiService plexiService;

        private ISpeechService speechService;

        private string[] cancelCommands = new string[] {"cancel", "nevermind", "never mind"};

        private string[] cancelMessages = new string[] { "ok then.", "as you wish.", "very well." };

        private bool showAppBar;

        protected bool disableSpeech = false;

        protected FrameworkElement debugger;

        public ViewBase(bool showAppBar = true)
        {
            this.showAppBar = showAppBar;
            
            try
            {
                // SystemTray.ProgressIndicator = new ProgressIndicator();

                // SystemTray.ProgressIndicator.IsIndeterminate = true;
                if (plexiService == null)
                {
                    plexiService = ViewModelLocator.GetServiceInstance<IPlexiService>();
                }

                if (speechService == null)
                {
                    speechService = ViewModelLocator.GetServiceInstance<ISpeechService>();
                }               
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            RegisterListeners();

            CreateApplicationBar(); 

            AddDebugger();

            //AddVerifyPrompt();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UnRegisterListeners();

            base.OnNavigatedFrom(e);
        }

        private static string ProfanityFilter(Match match)
        {
            string replacement = "";

            for (var i = 1; i < match.Groups.Count; i++)
            {
                for (var j = 0; j < match.Groups[i].Length; j++)
                {
                    replacement += "*";
                }
            }

            return replacement;
        }

        private void RegisterListeners()
        {
            // listen for http request progress message
            Messenger.Default.Register<ProgressMessage>(this, InProgress);
            
            // TODO: remove this listener. All speech to go directly to speechService
            // listen for any show messages
            Messenger.Default.Register<ShowMessage>(this, Speak);
        }

        private void UnRegisterListeners()
        {
            Messenger.Default.Unregister<ProgressMessage>(this, InProgress);
            Messenger.Default.Unregister<ShowMessage>(this, Speak);
        }

        protected void CreateApplicationBar()
        {
            if (ApplicationBar == null && showAppBar == true)
            {
                ApplicationBar = new ApplicationBar();
                //ApplicationBar.BackgroundColor = Colors.Black;
             
                // mic button
                var micBtn = new ApplicationBarIconButton()
                {
                    IconUri = new Uri("/Assets/microphone.png", UriKind.Relative),
                    Text = "speak",
                    IsEnabled = true
                };

                micBtn.Click += Microphone_Click;

                ApplicationBar.Buttons.Add(micBtn);

                // logout menu item
                var logout = new ApplicationBarMenuItem()
                {
                    Text = "Logout"
                };

                logout.Click += (s, e) =>
                    {
                        plexiService.LogoutUser();
                        NavigationService.Navigate(ViewModelLocator.RegistrationUri);
                    };

                ApplicationBar.MenuItems.Add(logout);

                // settings menu item
                var settings = new ApplicationBarMenuItem()
                {
                    Text = "Settings"
                };

                settings.Click += (s, e) =>
                {
                    NavigationService.Navigate(ViewModelLocator.SettingsPageUri);
                };

                ApplicationBar.MenuItems.Add(settings);

                // alarms menu item
                var alarms = new ApplicationBarMenuItem()
                {
                    Text = "Alarms"
                };

                alarms.Click += (s, e) =>
                {
                    DetailsViewModel vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

                    vm.CurrentItem = new AlarmListViewModel();

                    NavigationService.Navigate(ViewModelLocator.DetailsPageUri);
                };

                ApplicationBar.MenuItems.Add(alarms);

                // reminders menu item
                var reminders = new ApplicationBarMenuItem()
                {
                    Text = "Reminders"
                };

                reminders.Click += (s, e) =>
                {
                    ReminderListViewModel list = new ReminderListViewModel();

                    list.LoadReminders();

                    DetailsViewModel vm = ViewModelLocator.GetServiceInstance<DetailsViewModel>();

                    vm.CurrentItem = list;

                    NavigationService.Navigate(ViewModelLocator.DetailsPageUri);
                };

                ApplicationBar.MenuItems.Add(reminders);
            }
        }

        protected void AddCancelButton()
        {
            if (ApplicationBar.Buttons.Count == 1 && showAppBar == true)
            {
                var cancelBtn = App.Current.Resources["CancelBtn"] as ApplicationBarIconButton;

                cancelBtn.Click += Cancel_Click;

                ApplicationBar.Buttons.Add(cancelBtn);
            }
        }

        protected void RemoveCancelButton()
        {
            var cancelBtn = App.Current.Resources["CancelBtn"] as ApplicationBarIconButton;

            cancelBtn.Click -= Cancel_Click;

            ApplicationBar.Buttons.Remove(cancelBtn);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            RemoveCancelButton();
            CancelConversation();
        }

        protected async void Microphone_Click(object sender, EventArgs e)
        {
            plexiService.ResetTimer();

            if (speechService.isRecording == false)
            {
                var query = await speechService.PerformSpeechRecognition();

                if (query != null)
                {
                    ProcessQuery(query);
                }
            }
        }
        
        //TODO: remove. all speech, anywhere in the app, should go directly to the speechService
        private async void Speak(ShowMessage message)
        {
            await speechService.Speak(message.Speak);
        }

        private void CancelConversation()
        {
            speechService.CancelSpeak();

            plexiService.ClearContext();

            var vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

            vm.ClearDialog();

            var r = new Random();

            var ind = r.Next(0, cancelMessages.Length);

            //await speechService.Speak(cancelMessages[ind]);
        }

        private void ProcessQuery(string query)
        {
            try
            {
                var vm = ViewModelLocator.GetServiceInstance<MainViewModel>();

                if (Array.IndexOf(cancelCommands, query.Trim().ToLower()) != -1)
                {
                    CancelConversation();
                    return;
                }

                // add initial query to conversation list
                DialogType type = (plexiService.State == PlexiSDK.State.Uninitialized) ? DialogType.Query : DialogType.None;
               
                vm.AddDialog(DialogOwner.User, query, type);

                // send message to pleaseService to start the api adventure!!
                plexiService.Query(query);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void InProgress(ProgressMessage message)
        {
            InProgress(message.InProgress);

            if (ApplicationBar != null)
            {
                if (message.InProgress == true)
                {
                    foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                    {
                        button.IsEnabled = false;
                    }
                }

                if (message.InProgress == false)
                {
                    foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                    {
                        button.IsEnabled = true;
                    }
                }
            }
        }

        private void InProgress(bool inProgress)
        {
            try
            {
                var progressName = "PleaseRequestProgress";

                var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);
                var layoutRoot = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").FirstOrDefault();

                Canvas canvas;

                if (layoutRoot != null)
                {
                    var contentPanel = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "ContentPanel").FirstOrDefault();


                    if (inProgress == false)
                    {
                        canvas = currentPage.Descendants<Canvas>().Cast<Canvas>().Where(x => x.Name == progressName).FirstOrDefault();

                        if (canvas != null)
                        {
                            layoutRoot.Children.Remove(canvas);

                            if (contentPanel != null)
                            {
                                contentPanel.IsHitTestVisible = true;
                            }
                        }
                    }
                    else
                    {
                        var colSpan = layoutRoot.ColumnDefinitions.Count;
                        var rowSpan = layoutRoot.RowDefinitions.Count;

                        var deviceHeight = App.Current.Host.Content.ActualHeight;
                        var deviceWidth = App.Current.Host.Content.ActualWidth;

                        var background = new SolidColorBrush();
                        background.Color = Colors.Black;

                        canvas = new Canvas();
                        canvas.Name = progressName;
                        canvas.Height = deviceHeight;
                        canvas.Width = deviceWidth;
                        canvas.Background = background;
                        canvas.Opacity = .75;

                        if (colSpan > 0)
                        {
                            canvas.SetValue(Grid.ColumnSpanProperty, colSpan);
                        }

                        if (rowSpan > 0)
                        {
                            canvas.SetValue(Grid.RowSpanProperty, rowSpan);
                        }

                        var stack = new StackPanel();

                        stack.SetValue(Canvas.TopProperty, (deviceHeight / 2));

                        var bar = new ProgressBar();
                        bar.IsIndeterminate = true;
                        bar.IsEnabled = true;
                        bar.Width = deviceWidth;

                        var text = new TextBlock();
                        text.Text = "loading";
                        text.HorizontalAlignment = HorizontalAlignment.Center;

                        stack.Children.Add(bar);
                        stack.Children.Add(text);

                        canvas.Children.Add(stack);

                        layoutRoot.Children.Add(canvas);

                        if (contentPanel != null)
                        {
                            contentPanel.IsHitTestVisible = false;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(String.Format("InProgress Error: {0}", err.Message));
            }
        }

        #region beta verify prompt
        private void AddVerifyPrompt()
        {
            string betaKey = "StremorBetaTestKey";

            // check database if we already have credentials
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(betaKey))
            {
                // show verify control if no credentials are found
                var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);
                var layoutRoot = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").Single();

                if (layoutRoot != null)
                {
                    VerifyPrompt verifyPrompt = new VerifyPrompt();

                    var deviceHeight = App.Current.Host.Content.ActualHeight;
                    var deviceWidth = App.Current.Host.Content.ActualWidth;

                    var colSpan = layoutRoot.ColumnDefinitions.Count;
                    var rowSpan = layoutRoot.RowDefinitions.Count;

                    verifyPrompt.Height = deviceHeight;
                    verifyPrompt.Width = deviceWidth;

                    if (colSpan > 0)
                    {
                        verifyPrompt.SetValue(Grid.ColumnSpanProperty, colSpan);
                    }

                    if (rowSpan > 0)
                    {
                        verifyPrompt.SetValue(Grid.RowSpanProperty, rowSpan);
                    }

                    verifyPrompt.Closed += (s, e) => 
                        {
                            // save credentials to database
                            Dictionary<string, string> beta = new Dictionary<string,string>();

                            beta.Add("email", e.email);
                            beta.Add("code", e.code);

                            settings.Add(betaKey, beta);

                            // remove prompt from page
                            layoutRoot.Children.Remove(verifyPrompt); 
                        };

                    layoutRoot.Children.Add(verifyPrompt);
                }
            }

        }
        #endregion

        #region debug helpers
        private void AddDebugger()
        {
            if (debugger == null && ApplicationBar != null)
            {
                var currentPage = (App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage;

                var root = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").FirstOrDefault();

                if (root != null)
                {
                    var definition = new RowDefinition();
                    definition.Height = GridLength.Auto;
                    root.RowDefinitions.Add(definition);


                    // create appbar menu item
                    var input = new ApplicationBarMenuItem()
                    {
                        Text = "Show Input"
                    };

                    input.Click += MenuItem_Click;

                    // attach menu item to appbar
                    ApplicationBar.MenuItems.Add(input);

                    // create input scope
                    InputScope scope = new InputScope();
                    InputScopeName name = new InputScopeName();

                    name.NameValue = InputScopeNameValue.Text;
                    scope.Names.Add(name);

                    // create textbox
                    var textBox = new TextBox();
                    textBox.Name = "ManualInput";
                    textBox.Width = 480;
                    textBox.Visibility = Visibility.Collapsed;
                    textBox.KeyDown += OnKeyDown;
                    textBox.InputScope = scope;
                    //textBox.Text = "how much does it cost to live here";

                    // create stackpanel
                    debugger = new StackPanel();
                    debugger.SetValue(Grid.RowProperty, (root.RowDefinitions.Count - 1));
                    (debugger as StackPanel).Background = new SolidColorBrush(Colors.DarkGray);
                    (debugger as StackPanel).Orientation = System.Windows.Controls.Orientation.Horizontal;
                    // add textbox to stackpanel
                    (debugger as StackPanel).Children.Add(textBox);

                    // add stack panel to page
                    root.Children.Add(debugger);
                }
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var currentPage = (App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage;

            var input = currentPage.Descendants<TextBox>().Cast<TextBox>().Where(x => x.Name == "ManualInput").FirstOrDefault();

            if (input != null)
            {
                var item = (sender as ApplicationBarMenuItem);

                if (input.Visibility.Equals(Visibility.Collapsed))
                {
                    input.Visibility = Visibility.Visible;
                    item.Text = "Hide Input";
                    disableSpeech = true;
                }
                else
                {
                    input.Visibility = Visibility.Collapsed;
                    item.Text = "Show Input";
                    disableSpeech = false;
                }
            }
        }
        
        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var input = sender as TextBox; 

            if (e.Key.Equals(System.Windows.Input.Key.Enter))
            {
                var testInput = input.Text;

                input.Text = String.Empty;

                disableSpeech = true;

                ProcessQuery(testInput);
            }
        }
        #endregion

        #region IDialogSevice Implementation
        public virtual void ShowError(string message, string title)
        {
            Debug.WriteLine(message);
        }

        public virtual void ShowError(Exception error, string title)
        {
            Debug.WriteLine(error.Message);
        }

        public virtual void ShowMessageBox(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        public virtual void ShowMessage(string message, string title)
        {
            MessageBox.Show(message);
        }
        #endregion
    }
}
