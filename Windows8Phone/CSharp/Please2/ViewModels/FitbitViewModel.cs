using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using GalaSoft.MvvmLight.Command;

using Please2.Models;

namespace Please2.ViewModels
{
    public class FitbitViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
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

        // keep this around as a working command example
        public RelayCommand ChartTap { get; set; }

        public FitbitViewModel()
        {
            ChartTap = new RelayCommand(ChartTapHandler);
        }

        public void ChartTapHandler()
        {
            Debug.WriteLine("chart tap handler");
        }
    }
}
