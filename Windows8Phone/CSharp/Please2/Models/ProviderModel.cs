using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Util;

namespace Please2.Models
{
    public class ProviderModel : ModelBase
    {
        public AccountType name { get; set; }

        private AccountStatus _status;
        public AccountStatus status 
        { 
            get { return _status; } 
            set{ 
                _status = value; 
                NotifyPropertyChanged("status");
            } 
        }

        public bool isEnabled { get; set; }

        public ProviderModel()
        {
        }

        public ProviderModel(AccountType name, AccountStatus status)
        {
            this.name = name;
            this.status = status;
        }

        public ProviderModel(AccountType name, AccountStatus status, bool isEnabled)
        {
            this.name = name;
            this.status = status;
            this.isEnabled = isEnabled;
        }
    }
}
