using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiSDK.Models
{
    public class StateModel
    {
        public State State { get; set; }
   
        public object Response { get; set; }

        public StateModel(State state, object response)
        {
            Set(state, response);
        }

        public void Set(State state, object response)
        {
            this.Response = response;
            this.State = state;
        }

        public void Reset()
        {
            this.Response = null;
            this.State = State.Uninitialized;
        }
    }
}
