using System;

using PlexiSDK.Models;

namespace PlexiSDK.Events
{
    public class ActorEventArgs : EventArgs
    {
        public ClassifierModel data;

        public bool handled { get; set; }

        public ActorEventArgs(ClassifierModel data)
        {
            this.data = data;
        }
    }
}
