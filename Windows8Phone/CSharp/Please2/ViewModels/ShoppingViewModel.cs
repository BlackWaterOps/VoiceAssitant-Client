using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;

namespace Please2.ViewModels
{
    public class ShoppingViewModel : ViewModelBase
    {
        private List<ShoppingModel> _shoppingResults;
        public List<ShoppingModel> ShoppingResults
        {
            get { return _shoppingResults; }
            set { _shoppingResults = value; }
        }

        // type: amazon, bestbuy, walmart, etc.
        private string _shoppingType;
        public string ShoppingType
        {
            get { return _shoppingType; }
            set { _shoppingType = value; }
        }
    }
}
