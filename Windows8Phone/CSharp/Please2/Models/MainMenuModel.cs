using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Please2.Models
{
    public class MainMenuModel : ModelBase
    {
        public string background { get; set; }
        public string icon { get; set; }
        public string title { get; set; }
        public string page { get; set; }
        public bool isIntent { get; set; }

        // optional
        public object detail { get; set; }
    }
}
