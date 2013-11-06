using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plexi.Models
{
    public class StateModel : ModelBase
    {
        private string state;
        public string State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }

        public object Response { get; set; }

        public StateModel()
        {

        }

        public StateModel(string state, object response)
        {
            Set(state, response);
        }

        public void Set(string state, object response)
        {
            this.Response = response;
            this.State = state;
        }

        public void Reset()
        {
            this.Response = null;
            this.state = null;
        }
    }
}
