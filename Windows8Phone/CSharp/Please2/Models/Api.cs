using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Please2.Models
{
    public class ErrorModel
    {
        public string msg { get; set; }
        public string message { get; set; }
        public int code { get; set; }
    }

    public class ShowModel
    {
        public Dictionary<string, object> simple { get; set; }
        public Dictionary<string, object> structured { get; set; }
    }

    public class ClassifierModel
    {
        public string model { get; set; }
        public string action { get; set; }
        public Dictionary<string, object> payload { get; set; }

        // optional
        public List<string> project { get; set; }

        public ErrorModel error { get; set; }
    }

    public class DisambiguatorModel
    {
        public object payload { get; set; }
        public string type { get; set; }
        public Newtonsoft.Json.Linq.JArray candidates;
        public Dictionary<string, object> device_info { get; set; }
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

        public ErrorModel error { get; set; }
    }

    public class ActorModel
    {
        public string speak { get; set; }
        public ShowModel show { get; set; }

        public ErrorModel error { get; set; }
    }
}
