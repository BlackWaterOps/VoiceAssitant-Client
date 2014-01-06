using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Util;

namespace Please2.ViewModels
{
    public class ClockViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public ColorScheme Scheme { get { return ColorScheme.Default; } }

        private DateTime time;
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
                RaisePropertyChanged("Time");
            }
        }
    }
}
