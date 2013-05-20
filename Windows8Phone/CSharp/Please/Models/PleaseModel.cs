using System;
using System.Collections.Generic;
using System.Net;
namespace Please.Models
{
    public class Device
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public double timestamp { get; set; }
        public int timeoffset { get; set; }
    }

    public class Trigger
    {
        public Dictionary<string, object> payload { get; set; }
        public string action { get; set; }
    }

    public class Context
    {
        public string status { get; set; }
        public string query_type { get; set; }
        public Device device { get; set; }
    }

    public class PleaseModel
    {
        public Trigger trigger { get; set; }
        public Context context { get; set; }
        public string speak { get; set; }
        public string show { get; set; }
    }
}
