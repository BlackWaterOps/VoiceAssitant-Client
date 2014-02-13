using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;

using Newtonsoft.Json.Linq;

using PlexiVoice.Models;
using PlexiVoice.Util;
namespace PlexiVoice.ViewModels
{
    public class StockViewModel : ViewModelBase, IViewModel
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

        public void Load(string templateName, Dictionary<string, object> structured)
        {
            StockData = ((JToken)structured["item"]).ToObject<StockModel>();

            var direction = stockData.share_price_direction;

            if (direction == "down")
            {
                directionSymbol = "\uf063"; // arrow down
                //directionColor = "#dc143c"; // red
                directionColor = "#b22222"; // firebrick
            }
            else if (direction == "up")
            {
                directionSymbol = "\uf062"; // arrow up
                directionColor = "#006400"; // darkgreen
            }
        }
    }
}
