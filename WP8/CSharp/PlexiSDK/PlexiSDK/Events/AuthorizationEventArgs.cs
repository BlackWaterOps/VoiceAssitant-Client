using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiSDK.Events
{
    public class AuthorizationEventArgs : EventArgs
    {
        public string model;

        public AuthorizationEventArgs(string model)
        {
            this.model = model;
        }

        public AuthorizationEventArgs()
        {

        }
    }
}
