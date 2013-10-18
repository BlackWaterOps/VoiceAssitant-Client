using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class ProgressMessage
    {
        public ProgressMessage(bool inProgress)
        {
            InProgress = inProgress;
        }

        public bool InProgress { get; private set; }
        
        public string LoadingText { get; private set; }
    }

    public class ShowMessage
    {
        public ShowMessage(string text, string speak, string link = null)
        {
            Text = text;
            Speak = speak;
            Link = link;
        }

        public string Text { get; private set; }

        public string Speak { get; private set; }

        public string Link { get; private set; }
    }
}
