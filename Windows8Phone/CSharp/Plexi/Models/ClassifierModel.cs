using System;
using System.Collections.Generic;

namespace Plexi.Models
{
    public class ClassifierModel
    {
        public string model { get; set; }
        public string action { get; set; }
        public Dictionary<string, object> payload { get; set; }

        // optional
        public List<string> project { get; set; }

        public ErrorModel error { get; set; }
    }
}
