using System;

namespace PlexiSDK.Models
{
    public class ActorModel
    {
        public string speak { get; set; }
        public ShowModel show { get; set; }

        public ErrorModel error { get; set; }
    }
}
