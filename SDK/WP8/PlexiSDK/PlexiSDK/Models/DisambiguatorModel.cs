using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace PlexiSDK.Models
{
    public class DisambiguatorModel
    {
        public object payload { get; set; }
        public string type { get; set; }
        public JArray candidates;
        public Dictionary<string, object> device_info { get; set; }
    }
}
