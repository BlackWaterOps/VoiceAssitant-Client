using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexiVoice.Util;

namespace PlexiVoice.Models
{
    public class ProviderModel : ModelBase
    {
        public long id { get; set; }

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

        public ProviderModel()
        {
        }

        public ProviderModel(AccountType name, AccountStatus status)
        {
            this.name = name;
            this.status = status;
        } 

        public ProviderModel(long id, AccountType name, AccountStatus status)
        {
            this.id = id;
            this.name = name;
            this.status = status;
        }
    }
}
