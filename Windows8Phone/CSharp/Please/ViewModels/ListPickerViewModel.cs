using System;
using System.Net;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace Please.ViewModels
{
    public class ListPickerViewModel : ViewModelBase
    {
        private string _searchTerm;
        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                _searchTerm = value;
                NotifyPropertyChanged("SearchTerm");
            }
        }

        private ObservableCollection<string> _theList;
        public ObservableCollection<string> TheList
        {
            get
            {
                return _theList;
            }
            set
            {
                _theList = value;
                //NotifyPropertyChanged("TheList");
            }
        }

        // load collection for use in the view
        public void LoadList(List<string> data)
        {            
            TheList = new ObservableCollection<string>();

            foreach (string item in data)
            {
                TheList.Add(item);
            }
        }
    }
}
