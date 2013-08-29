using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class DisambiguatorModel
    {
        public object payload { get; set; }
        public List<string> types { get; set; }
        public string type { get; set; }
        public Dictionary<string, object> device_info { get; set; }    
    }
}
