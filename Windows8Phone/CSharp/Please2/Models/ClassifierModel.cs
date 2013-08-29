using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class ClassifierModel
    {
        public string model { get; set; }
        public string action { get; set; }
        public Dictionary<string, object> payload { get; set; }

        // optional
        public List<string> project { get; set; }
    }
}
