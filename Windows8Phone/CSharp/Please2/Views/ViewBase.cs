using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;

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
        // Speech handling
        private SpeechSynthesizer synthesizer;

        private SpeechRecognizerUI recognizer;

        private IAsyncOperation<SpeechRecognitionUIResult> recoOperation;

        protected bool disableSpeech = false;

        // allow sub classes to interact/alter the appbar
        protected ApplicationBar applicationBar;

        TextBox debuggerTextBox;

        public ViewBase()
        {
            Debug.WriteLine(this);

            try
            {
                Debug.WriteLine(this);
                // SystemTray.ProgressIndicator = new ProgressIndicator();

                // SystemTray.ProgressIndicator.IsIndeterminate = true;

                if (synthesizer == null)
                {
                    synthesizer = new SpeechSynthesizer();
                }

                if (recognizer == null)
                {
                    recognizer = new SpeechRecognizerUI();
                    recognizer.Settings.ReadoutEnabled = false;
                    recognizer.Settings.ShowConfirmation = false;
                }

                if (applicationBar == null)
                {
                    CreateApplicationBar();
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

            var debuggerBox = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).Descendants<TextBox>().Cast<TextBox>().Where( x => x.Name == "ManualInput");

            debuggerTextBox = (debuggerBox.Count() > 0) ? debuggerBox.Single() : null;

            //InProgress(true);
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
            Messenger.Default.Register<ProgressMessage>(this, InProgress);
            Messenger.Default.Register<ShowMessage>(this, Speak);
        }

        private void UnRegisterListeners()
        {
            Messenger.Default.Unregister<ProgressMessage>(this, InProgress);
            Messenger.Default.Unregister<ShowMessage>(this, Speak);
        }

        protected void CreateApplicationBar()
        {
            applicationBar = new ApplicationBar();

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

        protected void Microphone_Click(object sender, EventArgs e)
        {
            PerformSpeechRecognition();
        }

        protected async void PerformSpeechRecognition()
        {
            synthesizer.CancelAll();

            if (recoOperation != null && recoOperation.Status == AsyncStatus.Started)
            {
                recoOperation.Cancel();
            }

            try
            {
                recoOperation = recognizer.RecognizeWithUIAsync();

                //micBtn.IsEnabled = false;

                var recoResult = await recoOperation;

                // reset the mic button if the user cancels the recognition gui
                if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Cancelled)
                {
                    //micBtn.IsEnabled = true;
                }
                else if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    string query = recoResult.RecognitionResult.Text;

                    // replace profanity text
                    query = Regex.Replace(query, @"<profanity>(.*?)</profanity>", new MatchEvaluator(ProfanityFilter), RegexOptions.IgnoreCase);

                    // is this check needed in a succeeded state
                    if (recoResult.RecognitionResult.TextConfidence == SpeechRecognitionConfidence.Rejected)
                    {
                        //Say("please", "I didn't quite catch that. Can you say it again?");
                    }
                    else
                    {
                        //TODO: conditional is needed
                        // check if speak is in response to a task filter ie. narrow down results for a phone task like email or sms
                        // if (this.GetType() == typeof(ContactList))

                        ProcessQuery(query);
                    }
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {

            }
            catch (Exception err)
            {
                //MicrophoneBtn.IsEnabled = true;

                const int privacyPolicyHResult = unchecked((int)0x80045509);

                if (err.HResult == privacyPolicyHResult)
                {
                    MessageBox.Show("To run this sample, you must first accept the speech privacy policy. To do so, navigate to Settings -> speech on your phone and check 'Enable Speech Recognition Service' ");
                }
                else
                {
                    Debug.WriteLine(err.Message);
                    return;
                }
            }
        }

        private void Speak(ShowMessage message)
        {
            Speak(message.Speak);
        }

        public async void Speak(string type, string speak = "")
        {
            try
            {
                // say response
                if (type.ToLower() == "please" && speak != "" && disableSpeech == false)
                {
                    await synthesizer.SpeakTextAsync(speak);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        public async void Speak(string speak = "")
        {
            try
            {
                if (speak != "" && disableSpeech == false)
                {
                    await synthesizer.SpeakTextAsync(speak);
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void ProcessQuery(string query)
        {
            if (!SimpleIoc.Default.IsRegistered<ConversationViewModel>())
            {
                SimpleIoc.Default.Register<ConversationViewModel>();
            }

            var convo = SimpleIoc.Default.GetInstance<ConversationViewModel>();

            // add initial query to conversation list
            convo.AddDialog("user", query);

            // send message to viewmodel to start the api adventure!!
            Messenger.Default.Send<QueryMessage>(new QueryMessage(query));
        }

        private void InProgress(ProgressMessage message)
        {
            InProgress(message.InProgress);
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
        protected void AddDebugTextBox()
        {
            if (debuggerTextBox != null)
            {
                var input = new ApplicationBarMenuItem()
                {
                    Text = "Show Input"
                };

                input.Click += MenuItem_Click;

                ApplicationBar.MenuItems.Add(input);
            }
        }

        protected void MenuItem_Click(object sender, EventArgs e)
        {
            if (debuggerTextBox != null)
            {
                var item = (sender as ApplicationBarMenuItem);

                if (debuggerTextBox.Visibility.Equals(Visibility.Collapsed))
                {
                    debuggerTextBox.Visibility = Visibility.Visible;
                    item.Text = "Hide Input";
                    disableSpeech = true;
                }
                else
                {
                    debuggerTextBox.Visibility = Visibility.Collapsed;
                    item.Text = "Show Input";
                    disableSpeech = false;
                }
            }
        }
        
        protected virtual void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
