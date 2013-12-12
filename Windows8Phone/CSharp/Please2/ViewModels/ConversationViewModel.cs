using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Phone.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Please2.Models;
using Please2.Util;

using Plexi;
namespace Please2.ViewModels
{
    public class ConversationViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private const string phrase = "how may I help you?";

        public string Scheme { get { return "Default"; } }

        public DateTime SubTitle
        {
            get { return DateTime.Now; }
        }

        private ObservableCollection<DialogModel> dialogList;
        public ObservableCollection<DialogModel> DialogList
        {
            get { return dialogList; }
            set
            {
                dialogList = value;
                RaisePropertyChanged("DialogList"); 
            }
        }

        INavigationService navigationService;

        IPlexiService plexiService;

        ISpeechService speechService;

        public ConversationViewModel(INavigationService navigationService, IPlexiService plexiService, ISpeechService speechService)
        {
            this.navigationService = navigationService;

            this.plexiService = plexiService;

            this.speechService = speechService;

            Messenger.Default.Register<ShowMessage>(this, Show);

            if (DialogList == null)
            {
                DialogList = new ObservableCollection<DialogModel>();
            }

            //AddDummyDialog(15);
        }

        private void AddDummyDialog(int count)
        {
            for (int i = 0; i < count; i++)
            {
                string sender = (i % 2 == 0) ? "user" : "please";

                AddDialog(sender, "Your cells react to bacteria and viruses differently than mine. You don't get sick, I do.", ((i % 5 == 0) ? "http://www.plexisearch.com" : null));
            }
        }

        public async void AddOpeningDialog(bool speak = true)
        {
            if (DialogList.Count == 0)
            {
                AddDialog("please", phrase);

                if (speak == true)
                {
                    await speechService.Speak(phrase);
                }
            }
        }

        public void RemoveOpeningDialog()
        {
            var openingDialog = DialogList.Where(x => (string)x.message == phrase).FirstOrDefault();

            if (DialogList.Count == 1 && openingDialog != null)
            {
                DialogList.Clear();
            }
        }

        public void AddDialog(string sender, string message, string link = null)
        {
            Debug.WriteLine(message);

            if (DialogList == null)
            {
                DialogList = new ObservableCollection<DialogModel>();
            }
   
            var dialog = new DialogModel();

            dialog.sender = sender;
            dialog.message = message;
            dialog.link = link;
            
            DialogList.Add(dialog);
        }

        public void ClearDialog()
        {
            DialogList.Clear();

            if ((App.Current.RootVisual as PhoneApplicationFrame).CurrentSource.Equals(ViewModelLocator.ConversationPageUri))
            {
                AddOpeningDialog();
            }
        }

        private void Show(ShowMessage message)
        {
            var text = message.Text;
            var link = message.Link;

            if (text != null)
            {
                AddDialog("please", text, link);
            }
        }
    }
}
