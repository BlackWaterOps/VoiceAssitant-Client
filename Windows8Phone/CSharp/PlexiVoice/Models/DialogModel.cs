using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using PlexiVoice.Util;

namespace PlexiVoice.Models
{
    public class DialogModel : ModelBase
    {
        public DialogType type { get; set; }

        public DialogOwner sender { get; set; }
        
        public object message { get; set; }
        
        private string _link;
        public string link 
        { 
            get { return _link; }
            set 
            {
                _link = value;
                NotifyPropertyChanged("link");
            }
        }

        public DialogModel()
        {
        }

        public DialogModel(DialogOwner sender, object message, DialogType type)
        {
            this.sender = sender;
            this.message = message;
            this.type = type;
        }

        public DialogModel(DialogOwner sender, object message, string link, DialogType type)
        {
            this.sender = sender;
            this.message = message;
            this.link = link;
            this.type = type;
        }
    }
}
