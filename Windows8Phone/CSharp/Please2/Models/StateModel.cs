using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class StateModel : ModelBase
    {
        private string state;
        public string State { 
            get {
                return state;
            }

            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }

        // this will be a ResponderModel most of the time if not all the time
        // consider switching types
        public object Response { get; set; }
    }
}
