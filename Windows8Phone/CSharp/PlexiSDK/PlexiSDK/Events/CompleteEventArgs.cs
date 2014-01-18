using System;

using PlexiSDK.Models;

namespace PlexiSDK.Events
{
    public class CompleteEventArgs : EventArgs
    {
        public ActorModel actor;

        public CompleteEventArgs(ActorModel actor)
        {
            this.actor = actor;
        }
    }
}
