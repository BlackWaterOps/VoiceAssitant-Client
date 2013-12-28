using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Util
{
    // primary used for note list styles
    public enum ListStyle
    {
        None = 0, Ordered = 1, Unordered = 2
    }

    // used for accounts on settings view
    public enum AccountStatus
    {
        NotConnected = 0, Connected = 1
    }

    public enum AccountType
    {
        None, Google, Facebook, Fitbit
    }
}
