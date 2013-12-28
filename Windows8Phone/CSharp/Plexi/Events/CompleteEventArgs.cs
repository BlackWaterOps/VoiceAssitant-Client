using System;

using Plexi.Models;

namespace Plexi.Events
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
