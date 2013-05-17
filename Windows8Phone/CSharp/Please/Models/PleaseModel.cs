using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please.Models
{
    /*
    class Payload
    {
        public string? url { get; set; }
        public int? phone { get; set; }
        public int? seconds { get; set; }
        public string? ticker { get; set; }
        public string? message { get; set; }
    }
    */

    public class Trigger
    {
        public Dictionary<string, object> payload { get; set; }
        public string action { get; set; }
    }

    public class PleaseModel
    {
        public Trigger trigger { get; set; }
        public string speak { get; set; }
    }
}
