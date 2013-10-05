using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;

namespace Please2.ViewModels
{
    class FitbitViewModel : GalaSoft.MvvmLight.ViewModelBase
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

        public void AddTestData()
        {
            var testData = "{\"timeseries\": [{\"value\": \"231.47999572753906\", \"dateTime\": \"2013-08-29\"}, {\"value\": \"228.7100067138672\",\"dateTime\": \"2013-08-30\"},{\"value\": \"225.94000244140625\",\"dateTime\": \"2013-08-31\"},{\"value\": \"240.1699981689453\",\"dateTime\": \"2013-09-01\"},{\"value\": \"230.39999389648438\",\"dateTime\": \"2013-09-02\"},{\"value\": \"217.6300048828125\",\"dateTime\": \"2013-09-03\"},{\"value\": \"225.86000061035156\",\"dateTime\": \"2013-09-04\"},{\"value\": \"212.08999633789062\",\"dateTime\": \"2013-09-05\"},{\"value\": \"215.32000732421875\",\"dateTime\": \"2013-09-06\"},{\"value\": \"206.5500030517578\",\"dateTime\": \"2013-09-07\"},{\"value\": \"203.7899932861328\",\"dateTime\": \"2013-09-08\"},{\"value\": \"201.0\",\"dateTime\": \"2013-09-09\"}]}";

            try
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FitbitModel>(testData);

                Points = data.timeseries;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
}
