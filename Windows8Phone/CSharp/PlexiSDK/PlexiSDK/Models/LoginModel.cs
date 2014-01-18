using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiSDK.Models
{
    public class LoginModel
    {
        public string token { get; set; }
        public string username { get; set; }
        public ErrorModel error { get; set; }
    }
}
