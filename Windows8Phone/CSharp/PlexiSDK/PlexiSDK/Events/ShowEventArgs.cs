using System;

namespace PlexiSDK.Events
{
    public class ShowEventArgs : EventArgs
    {
        public string speak;
        public string show;
        public string link;
        public State status;

        public ShowEventArgs(string speak, string show, string link, State status)
        {
            this.speak = speak;
            this.show = show;
            this.link = link;
            this.status = status;
        }
    }
}
