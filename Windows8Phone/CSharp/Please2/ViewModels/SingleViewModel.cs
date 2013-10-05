using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.ViewModels
{
    class SingleViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public string SubTitle
        {
            get { return DateTime.Now.ToString("dddd, MMMM d, yyyy @ h:mm tt"); }
        }

        private object singleItem;
        public object SingleItem
        {
            get { return singleItem; }
            set
            {
                singleItem = value;
                RaisePropertyChanged("SingleItem");
            }
        }

        private List<object> multiItems;
        public List<object> MultiItems
        {
            get { return multiItems; }
            set
            {
                multiItems = value;
                RaisePropertyChanged("MultiItems");
            }
        }
    }
}
