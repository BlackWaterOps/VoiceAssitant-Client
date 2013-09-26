using System;
using System.ComponentModel;
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
        //INavigationService navigationService { get; set; }

        void HandleUserInput(string query);
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
    }
}
