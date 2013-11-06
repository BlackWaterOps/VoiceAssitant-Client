using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Please2.Models;
using Please2.Util;

namespace Please2.ViewModels
{
    public class StockViewModel : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        private StockModel stockData;
        public StockModel StockData
        {
            get { return stockData; }
            set
            {
                stockData = value;
                RaisePropertyChanged("StockData");
            }
        }

        private string directionColor;
        public string DirectionColor
        {
            get { return directionColor; }
            set { directionColor = value; }
        }

        private string directionSymbol;
        public string DirectionSymbol
        {
            get { return directionSymbol; }
            set { directionSymbol = value; }
        }

        public Dictionary<string, object> Populate(string templateName, Dictionary<string, object> structured)
        {
            var ret = new Dictionary<string, object>();

            if (structured.ContainsKey("item"))
            {
                stockData = ((JToken)structured["item"]).ToObject<StockModel>();

                var direction = stockData.share_price_direction;

                if (direction == "down")
                {
                    directionSymbol = "\uf063"; // arrow down
                    directionColor = "#dc143c"; // red
                }
                else if (direction == "up")
                {
                    directionSymbol = "\uf062"; // arrow up
                    directionColor = "#008000"; // green
                }

                ret.Add("title", "stock");
                ret.Add("subtitle", stockData.name + "(" + stockData.symbol + ")");
            }
            return ret;
        }
    }
}
