using System;

namespace Plexi.Events
{
    public class ShowEventArgs : EventArgs
    {
        public string speak;
        public string show;
        public string link;
        public string status;

        public ShowEventArgs(string speak, string show, string link, string status)
        {
            this.speak = speak;
            this.show = show;
            this.link = link;
            this.status = status;
        }
    }
}
