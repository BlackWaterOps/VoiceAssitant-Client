using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Util;

namespace Please2.Models
{
    public class ProviderModel
    {
        public string name { get; set; }
        
        public AccountStatus status { get; set; }

        public string endpointName { get; set; }

        public bool isEnabled { get; set; }

        public ProviderModel()
        {
        }

        public ProviderModel(string name, AccountStatus status, string endpointName, bool isEnabled)
        {
            this.name = name;
            this.status = status;
            this.endpointName = endpointName;
            this.isEnabled = isEnabled;
        }
    }
}
