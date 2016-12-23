using System;

using PlexiSDK.Models;

namespace PlexiSDK.Events
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
