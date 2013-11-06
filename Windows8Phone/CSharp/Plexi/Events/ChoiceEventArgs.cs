using System;

using Plexi.Models;

namespace Plexi.Events
{
    public class ChoiceEventArgs : EventArgs
    {
        public ResponderModel results;

        public ChoiceEventArgs(ResponderModel results)
        {
            this.results = results;
        }
    }
}
