using System;

namespace PlexiSDK.Models
{
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
}
