using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Phone.UserData;

using Please2.Models;

using PlexiSDK.Models;
namespace Please2.Util
{
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;

        event NavigationFailedEventHandler NavigationFailed;

        event NavigatedEventHandler Navigated;

        void NavigateTo(Uri uri);

        void GoBack();
    }

    public interface IDialogService
    {
        void ShowError(string message, string title);

        void ShowError(Exception error, string title);

        void ShowMessageBox(string message, string title);

        void ShowMessage(string message, string title);
    }

    public interface ISpeechService
    {
        Task<string> PerformSpeechRecognition();

        void Speak(string type, string message);

        void CancelSpeak();

        Task Speak(string message);

        bool isRecording { get; }
    }
   
    public interface IViewModel
    {
        /// <summary>
        /// A Mechanism to load data in the view model before navigation
        /// </summary>
        /// <param name="payload"></param>
        Dictionary<string, object> Load(string templateName, Dictionary<string, object> structured);
    }

    public interface ITaskService
    {
        ClassifierModel MainContext { get; set; }

        /// <summary>
        /// Handle contact returned from Plexi Service
        /// </summary>
        /// <param name="payload"></param>
        void ComposeEmail(Dictionary<string, object> payload);

        /// <summary>
        /// Handle contact from internal lookup
        /// </summary>
        /// <param name="contact"></param>
        void ComposeEmail(Contact contact);

        /// <summary>
        /// Initiate an internal contact lookup 
        /// </summary>
        void ComposeEmail();

        /// <summary>
        /// Handle contact returned from Plexi Service
        /// </summary>
        /// <param name="payload"></param>
        void ComposeSms(Dictionary<string, object> payload);

        /// <summary>
        /// Handle contact from internal lookup
        /// </summary>
        /// <param name="contact"></param>
        void ComposeSms(Contact contact);

        /// <summary>
        /// Initiate an internal contact lookup 
        /// </summary>
        void ComposeSms();

        /// <summary>
        /// Handle contact returned from Plexi Service
        /// </summary>
        /// <param name="payload"></param>
        void PhoneCall(Dictionary<string, object> payload);

        /// <summary>
        /// Handle contact from internal lookup
        /// </summary>
        /// <param name="contact"></param>
        void PhoneCall(Contact contact);

        /// <summary>
        /// Initiate an internal contact lookup 
        /// </summary>
        void PhoneCall();

        void GetDirections(Dictionary<string, object> payload);

        void SetLocation(Dictionary<string, object> payload);

        void SetAppointment(Dictionary<string, object> payload);

        void ShowClock(Dictionary<string, object> payload);

        void SetAlarm(Dictionary<string, object> payload);

        void UpdateAlarm(Dictionary<string, object> payload);

        void SetReminder(Dictionary<string, object> payload);

        void UpdateReminder(Dictionary<string, object> payload);
    }
}
