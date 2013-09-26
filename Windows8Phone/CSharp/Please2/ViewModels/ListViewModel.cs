using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;

using Please2.Util;

namespace Please2.ViewModels
{
    public class ListViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string pageTitle;

        public string PageTitle
        {
            get { return pageTitle; }
            set 
            { 
                pageTitle = value;
                RaisePropertyChanged("PageTitle");
            }
        }
        
        private DataTemplate template;

        public DataTemplate Template
        {
            get { return template; }
            set
            {
                template = value;
                RaisePropertyChanged("Template");
            }
        }

        private IEnumerable<object> listResults;

        public IEnumerable<object> ListResults
        {
            get { return listResults; }
            set
            {
                listResults = value;
                RaisePropertyChanged("ListResults");
            }
        }

        public ListViewModel()
        {

        }

        public IEnumerable<T> CreateList<T>()
        {
            var myType = typeof(T);

            var listGenericType = typeof(IEnumerable<>);

            var list = listGenericType.MakeGenericType(myType);

            var ci = list.GetConstructor(new Type[] { });

            var typedList = (IEnumerable<T>)ci.Invoke(new object[] { });

            return typedList;
        }
    }
}
