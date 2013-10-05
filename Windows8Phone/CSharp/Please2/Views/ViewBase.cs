using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
            try
            {
                //SystemTray.ProgressIndicator = new ProgressIndicator();

                //SystemTray.ProgressIndicator.IsIndeterminate = true;

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

            //App.currentPage = e.Uri.ToString();

            var debuggerBox = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).Descendants<TextBox>().Cast<TextBox>().Where( x => x.Name == "ManualInput");

            debuggerTextBox = (debuggerBox.Count() > 0) ? debuggerBox.Single() : null;
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
            Messenger.Default.Register<ProgressMessage>(this, HandleProgressMessage);
            Messenger.Default.Register<ShowMessage>(this, HandleShowMessage);
        }

        private void UnRegisterListeners()
        {
            Messenger.Default.Unregister<ProgressMessage>(this, HandleProgressMessage);
            Messenger.Default.Unregister<ShowMessage>(this, HandleShowMessage);
        }

        private void HandleProgressMessage(ProgressMessage message)
        {
            if (SystemTray.ProgressIndicator != null)
            {
                SystemTray.ProgressIndicator.IsVisible = message.InProgress;
            }
        }

        private void HandleShowMessage(ShowMessage message)
        {
            Speak(message.Speak);
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
