using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Please2.Models;

namespace Please2.ViewModels
{
    public class StockViewModel : GalaSoft.MvvmLight.ViewModelBase
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

        public StockViewModel()
        {
            //StockTest();
        }

        private void StockTest()
        {
            var data = "{\"show\":{\"simple\":{\"text\":\"Facebook stock is trading at $49.19, down 2.17%\"},\"structured\":{\"item\":{\"opening_price\":50.46,\"low_price\":49.06,\"P/E Ratio\":227.51,\"share_price\":49.19,\"stock_exchange\":\"NasdaqNM\",\"trade_volume\":\"48.20 million\",\"52_week_high\":51.6,\"average_trade_volume\":\"70.12 million\",\"pe\":0.221,\"high_price\":50.72,\"share_price_change\":-1.09,\"market_cap\":\"119.80 billion\",\"5_day_moving_average\":43.542,\"symbol\":\"FB\",\"share_price_change_percent\":2.17,\"name\":\"Facebook\",\"yield\":\"N/A\",\"52_week_low\":18.8,\"share_price_direction\":\"down\"},\"template\":\"simple:stock\"}},\"speak\":\"Facebook stock is trading at $49.19, down 2.17%\"}";

            var actor = Newtonsoft.Json.JsonConvert.DeserializeObject<Please2.Models.ActorModel>(data);

            var show = actor.show;

            stockData = ((Newtonsoft.Json.Linq.JToken)show.structured["item"]).ToObject<StockModel>();

            var direction = stockData.share_price_direction;

            if (direction == "down")
            {
                directionSymbol = "\uf063";
                directionColor = "#dc143c";
            }
            else if (direction == "up")
            {
                directionSymbol = "\uf062";
                directionColor = "#008000";
            }
        }
    }
}
