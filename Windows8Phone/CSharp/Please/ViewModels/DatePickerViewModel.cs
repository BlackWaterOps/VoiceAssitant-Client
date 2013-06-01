using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please.ViewModels
{
    public class DatePickerViewModel : ViewModelBase
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

        private DateTime _defaultDate;
        public DateTime DefaultDate
        {
            get
            {
                return _defaultDate;
            }
            set
            {
                _defaultDate = value;
                NotifyPropertyChanged("DefaultDate");
            }
        }
    }
}
