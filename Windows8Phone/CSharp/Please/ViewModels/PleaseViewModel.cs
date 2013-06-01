using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please.Models;

namespace Please.ViewModels
{
    public class PleaseViewModel: ViewModelBase
    {
        // list of spoken and response text
        private ObservableCollection<DialogModel> _pleaseList;
        public ObservableCollection<DialogModel> PleaseList
        {
            get
            {
                return _pleaseList;
            }
            set
            {
                _pleaseList = value;
                NotifyPropertyChanged("PleaseList");
            }
        }

        public void AddDialog(string sender, string message, string link = "")
        {
            if (PleaseList == null)
            {
                PleaseList = new ObservableCollection<DialogModel>();
            }

            var dialog = new DialogModel();

            dialog.sender = sender;
            dialog.message = message;
            dialog.link = link;

            PleaseList.Add(dialog);
        }
    }
}
