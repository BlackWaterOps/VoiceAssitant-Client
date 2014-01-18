using System;

namespace PlexiSDK.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public string message;

        public ErrorEventArgs(string message)
        {
            this.message = message;
        }
    }
}
