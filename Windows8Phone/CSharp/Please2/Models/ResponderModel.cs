using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class ShowModel
    {
        public Dictionary<string, object> simple { get; set; }
        public Dictionary<string, object> structured { get; set; }
    }

    public class ResponderModel
    {
        public string status { get; set; }
        public string type { get; set; }
        public string field { get; set; }
        public ShowModel show { get; set; }
        public string speak { get; set; }

        public string followup { get; set; }
        public string actor { get; set; }

        public ClassifierModel data { get; set; }
    }
}
