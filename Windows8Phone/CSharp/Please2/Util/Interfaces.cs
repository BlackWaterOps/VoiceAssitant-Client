using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Navigation;

using Microsoft.Phone.UserData;

using Please2.Models;

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
        void PerformSpeechRecognition();

        void Speak(string type, string message);

        void Speak(string message);
    }

    public interface IPleaseService
    { 
        string OriginalQuery { get; }

        // TODO: (new name) void Query(string query);
        void HandleUserInput(string query);

        Task Auditor(ChoiceModel choice);
    }

    public interface IViewModel
    {
        Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured);
    }

    public interface ITaskService
    {
        ClassifierModel MainContext { get; set; }

        void ComposeEmail(Contact contact);
        void ComposeEmail();

        void ComposeSms(Contact contact);
        void ComposeSms();

        void PhoneCall(Contact contact);
        void PhoneCall();

        void GetDirections();

        void SetAppointment();

        void ShowClock();

        void SetAlarm();

        void SetReminder();
    }
}
