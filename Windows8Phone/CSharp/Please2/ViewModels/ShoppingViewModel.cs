using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;

namespace Please2.ViewModels
{
    public class ShoppingViewModel : NotificationBase
    {
        private List<ShoppingModel> shoppingResults;
        public List<ShoppingModel> ShoppingResults
        {
            get { return shoppingResults; }
            set { shoppingResults = value; }
        }

        // type: amazon, bestbuy, walmart, etc.
        private string shoppingType;
        public string ShoppingType
        {
            get { return shoppingType; }
            set { shoppingType = value; }
        }

        private string shoppingQuery;
        public string ShoppingQuery
        {
            get { return shoppingQuery; }
            set { shoppingQuery = value; }
        }

        public void SetShoppingResults(object items)
        {
            if (ShoppingResults == null)
                ShoppingResults = new List<ShoppingModel>();

            //ShoppingResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShoppingModel>>(SerializeData(items));
            ShoppingResults = (List<ShoppingModel>)items;
        }
    }
}
