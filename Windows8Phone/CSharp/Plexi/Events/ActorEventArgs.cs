using System;

using Plexi.Models;

namespace Plexi.Events
{
    public class ActorEventArgs : EventArgs
    {
        public ActorModel actor;

        public ActorEventArgs(ActorModel actor)
        {
            this.actor = actor;
        }
    }
}
