﻿using System;

namespace PlexiSDK.Events
{
    public class ProgressEventArgs : EventArgs
    {
        public bool inProgress;

        public ProgressEventArgs(bool inProgress)
        {
            this.inProgress = inProgress;
        }
    }
}
