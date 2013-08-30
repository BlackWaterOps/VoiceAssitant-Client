using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;

using Microsoft.Phone.Controls;

using Please2.Models;

namespace Please2.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private ObservableCollection<DialogModel> _dialogList;
        public ObservableCollection<DialogModel> DialogList
        {
            get { return _dialogList; }
            set
            {
                _dialogList = value;
                NotifyPropertyChanged("DialogList"); 
            }
        }

        public MainViewModel()
        {
            ObservableCollection<DialogModel> list = new ObservableCollection<DialogModel>();

            list.Add(new DialogModel() { 
                sender = "user",
                message = "user message"
            });

            list.Add(new DialogModel()
            {
                sender = "please",
                message = "please message"
            });

            list.Add(new DialogModel()
            {
                sender = "user",
                message = "Well, the way they make shows is, they make one show. That show's called a pilot. Then they show that show to the people who make shows, and on the strength of that one show they decide if they're going to make more shows. Some pilots get picked and become television programs. Some don't, become nothing. She starred in one of the ones that became nothing."
            });

            list.Add(new DialogModel()
            {
                sender = "please",
                message = "please message"
            });

            //DialogList = list;
        }

        // NOTE: might be able to change type for message to string later
        public void AddDialog(string sender, object message, string link = null)
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
