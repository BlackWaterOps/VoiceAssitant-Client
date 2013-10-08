using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Please2.Models;

namespace Please2.ViewModels
{
    public class FitbitViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private PointCollection pcollection;
        public PointCollection Pcollection
        {
            get { return pcollection; }
            set
            {
                pcollection = value;
                RaisePropertyChanged("Pcollection");
            }
        }
        
        private IEnumerable<object> points;
        public IEnumerable<object> Points
        {
            get { return points; }
            set 
            {
                points = value;
                RaisePropertyChanged("Points");
            }
        }

        private FitbitGoals goals;
        public FitbitGoals Goals
        {
            get { return goals; }
            set
            {
                goals = value;
                RaisePropertyChanged("Goals");
            }
        }
    }
}
