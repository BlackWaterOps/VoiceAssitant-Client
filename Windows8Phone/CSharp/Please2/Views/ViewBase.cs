﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using Newtonsoft.Json;

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using Please2.Models;
using Please2.Util;
using Please2.ViewModels;

using LinqToVisualTree;

namespace Please2.Views
{
    public class ViewBase : PhoneApplicationPage, IDialogService
    {
        private IPleaseService pleaseService;

        private ISpeechService speechService;

        private string[] cancelCommands = new string[] {"cancel", "nevermind", "never mind"};

        private string[] cancelMessages = new string[] { "ok then.", "as you wish.", "very well." };

        protected bool disableSpeech = false;

        // allow sub classes to interact/alter the appbar
        protected ApplicationBar applicationBar;

        protected FrameworkElement debugger;

        public ViewBase()
        {
            try
            {
                // SystemTray.ProgressIndicator = new ProgressIndicator();

                // SystemTray.ProgressIndicator.IsIndeterminate = true;
                if (pleaseService == null)
                {
                    pleaseService = ViewModelLocator.GetViewModelInstance<IPleaseService>();
                }

                if (speechService == null)
                {
                    speechService = ViewModelLocator.GetViewModelInstance<ISpeechService>();
                }

                CreateApplicationBar();
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

            CreatePageBackground();
            
            AddDebugger();
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
            // listen for any show messages
            Messenger.Default.Register<ShowMessage>(this, Speak);
        }

        private void UnRegisterListeners()
        {
            Messenger.Default.Unregister<ProgressMessage>(this, InProgress);
            Messenger.Default.Unregister<ShowMessage>(this, Speak);
        }

        protected void CreatePageBackground()
        {
            var bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("/Assets/plexi-big-black-720.png", UriKind.Relative));
            bg.Opacity = 0.30;
            bg.AlignmentX = AlignmentX.Center;
            bg.AlignmentY = AlignmentY.Bottom;
            bg.Stretch = Stretch.Uniform;

            var currentPage = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage);
            var layoutRoot = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").Single();

            if (layoutRoot != null)
            {
                layoutRoot.Background = bg;
            }
        }

        protected void CreateApplicationBar()
        {
            if (applicationBar == null)
            {
                applicationBar = new ApplicationBar();

                Debug.WriteLine("appbar count: " + applicationBar.Buttons.Count);

                var micBtn = new ApplicationBarIconButton()
                {
                    IconUri = new Uri("/Assets/microphone.png", UriKind.Relative),
                    Text = "speak",
                    IsEnabled = true
                };

                micBtn.Click += Microphone_Click;

                applicationBar.Buttons.Add(micBtn);

                ApplicationBar = applicationBar;
            }
        }

        protected void AddCancelButton()
        {
            if (applicationBar.Buttons.Count == 1)
            {
                var cancelBtn = App.Current.Resources["CancelBtn"] as ApplicationBarIconButton;

                cancelBtn.Click += Cancel_Click;

                applicationBar.Buttons.Add(cancelBtn);
            }
        }

        protected void RemoveCancelButton()
        {
            var cancelBtn = App.Current.Resources["CancelBtn"] as ApplicationBarIconButton;

            cancelBtn.Click -= Cancel_Click;

            applicationBar.Buttons.Remove(cancelBtn);
        }

        protected async void Cancel_Click(object sender, EventArgs e)
        {
            RemoveCancelButton();
            await CancelConversation();
        }

        protected async void Microphone_Click(object sender, EventArgs e)
        {
            pleaseService.ResetTimer();

            if (speechService.isRecording == false)
            {
                var query = await speechService.PerformSpeechRecognition();

                if (query != null)
                {
                    ProcessQuery(query);
                }
            }
        }

        private async void Speak(ShowMessage message)
        {
            await speechService.Speak(message.Speak);
        }

        private async Task CancelConversation()
        {
            speechService.CancelSpeak();

            pleaseService.ClearContext();

            var vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();

            vm.ClearDialog();

            var r = new Random();

            var ind = r.Next(0, cancelMessages.Length);

            //await speechService.Speak(cancelMessages[ind]);
        }

        private async void ProcessQuery(string query)
        {
            try
            {
                var vm = ViewModelLocator.GetViewModelInstance<ConversationViewModel>();

                if (Array.IndexOf(cancelCommands, query.Trim().ToLower()) != -1)
                {
                    await CancelConversation();
                    return;
                }

                // add initial query to conversation list
                vm.AddDialog("user", query);

                // send message to pleaseService to start the api adventure!!
                pleaseService.Query(query);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void InProgress(ProgressMessage message)
        {
            InProgress(message.InProgress);

            if (applicationBar != null)
            {
                if (message.InProgress == true)
                {
                    foreach (ApplicationBarIconButton button in applicationBar.Buttons)
                    {
                        button.IsEnabled = false;
                    }
                }

                if (message.InProgress == false)
                {
                    foreach (ApplicationBarIconButton button in applicationBar.Buttons)
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
                var layoutRoot = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").Single();

                Canvas canvas;

                if (layoutRoot != null)
                {
                    var contentPanel = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "ContentPanel").Single();

                    if (inProgress == false)
                    {
                        canvas = currentPage.Descendants<Canvas>().Cast<Canvas>().Where(x => x.Name == progressName).Single();

                        layoutRoot.Children.Remove(canvas);

                        if (contentPanel != null)
                        {
                            contentPanel.IsHitTestVisible = true;
                        }
                    }
                    else
                    {
                        var colSpan = layoutRoot.ColumnDefinitions.Count;
                        var rowSpan = layoutRoot.RowDefinitions.Count;

                        var deviceHeight =  App.Current.Host.Content.ActualHeight;
                        var deviceWidth = App.Current.Host.Content.ActualWidth;
                        /*
                        var loadingScreen = App.Current.Resources["LoadingScreen"];

                        if (loadingScreen != null)
                        {
                            canvas = (loadingScreen as Canvas);

                            canvas.Width = deviceWidth;
                            canvas.Height = deviceHeight;

                            var border = canvas.FindName("LoadingScreenBorder") as Border;
                            
                            var stack = canvas.FindName("LoadingScreenStackPanel") as StackPanel;

                            if (stack != null)
                            {
                                stack.SetValue(Canvas.TopProperty, (deviceHeight / 2));
                            }

                            layoutRoot.Children.Add(canvas);

                            VisualStateManager.GoToState(this, "Enabled", true);
                        }
                        */
                       
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
                Debug.WriteLine(err.Message);
            }
        }

        #region debug helpers
        protected void AddDebugger()
        {
            if (debugger == null)
            {
                var currentPage = (App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage;

                var root = currentPage.Descendants<Grid>().Cast<Grid>().Where(x => x.Name == "LayoutRoot").FirstOrDefault();

         
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
                applicationBar.MenuItems.Add(input);

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

        protected void MenuItem_Click(object sender, EventArgs e)
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
        
        protected void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
