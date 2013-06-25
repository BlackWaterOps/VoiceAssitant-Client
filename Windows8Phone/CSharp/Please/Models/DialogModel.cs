using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Please.Models
{
    public class DialogModel : ModelBase
    {
        public string sender { get; set; }
        public object message { get; set; }
        private string _link;
        public string link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
                NotifyPropertyChanged("link");
            }
        }

        public DataTemplate template { get; set; }
    }
}
