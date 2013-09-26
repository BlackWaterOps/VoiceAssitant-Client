using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class ConversationViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public string SubTitle
        {
            get { return DateTime.Now.ToString("dddd, MMMM d, yyyy @ h:mm tt"); }
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
        }
        
        /*
        public ConversationViewModel()
        {
        }
        */

        private void Show(ShowMessage message)
        {
            var text = message.Text;
            var link = message.Link;

            AddDialog("please", text, link);
        }

        // we hit the show override so we must be showing please dialog
        /*
        private void Show(ShowModel showModel, string speak = "")
        {
            Debug.WriteLine("conversation show method");

            if (showModel.simple.ContainsKey("text"))
            {
                string show = (string)showModel.simple["text"];

                string link = null;

                if (showModel.simple.ContainsKey("link"))
                    link = (string)showModel.simple["link"];

                AddDialog("please", show, link);
            }
            else
            {
                Debug.WriteLine("conversation viewmodel: no simple text found");
            }
        }
        */

        // NOTE: might be able to change type for message to string later
        public void AddDialog(string sender, string message, string link = null)
        {
            if (DialogList == null)
            {
                DialogList = new ObservableCollection<DialogModel>();
            }
   
            var dialog = new DialogModel();

            dialog.sender = sender;
            dialog.message = message;

            if (link != null)
                dialog.link = link;

            DialogList.Add(dialog);
        }
    }
}
