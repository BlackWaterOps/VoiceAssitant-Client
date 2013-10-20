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

namespace Please2.ViewModels
{
    public class ConversationViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private const string phrase = "how may I help you?";

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

        IPleaseService pleaseService;

        public ConversationViewModel(INavigationService navigationService, IPleaseService pleaseService)
        {
            this.navigationService = navigationService;

            this.pleaseService = pleaseService;

            Messenger.Default.Register<ShowMessage>(this, Show);

            if (DialogList == null)
            {
                DialogList = new ObservableCollection<DialogModel>();
            }
        }

        public void AddOpeningDialog()
        {
            if (DialogList.Count == 0)
            {
                AddDialog("please", phrase);

                // only send message to viewbase. Not to ourself
                Messenger.Default.Send(new ShowMessage(null, phrase, null), "viewbase");
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
            Debug.WriteLine("viewmodel catch show message");
            var text = message.Text;
            var link = message.Link;

            if (text != null)
            {
                AddDialog("please", text, link);
            }
        }
    }
}
