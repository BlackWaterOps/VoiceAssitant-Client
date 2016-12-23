using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
    public class StockModel
    {
        private string ReduceText(string value)
        {
            if (String.IsNullOrEmpty(value) || value == "N/A")
            {
                return value;
            }

            value = value.Trim();

            String[] split = value.Split(' ');

            if (split.Length == 1)
            {
                return value;
            }
            
            string text = split[1].Substring(0, 1).ToUpper();

            return String.Format("{0}{1}", split[0] , text);
        }

        public string opening_price { get; set; }
        public string low_price { get; set; }
        public double share_price { get; set; }
        public string stock_exchange { get; set; }

        private string _trade_volume;
        public string trade_volume 
        {
            get { return _trade_volume; }
            set { _trade_volume = ReduceText(value); }
        }

        private string _average_trade_volume;
        public string average_trade_volume 
        {
            get { return _average_trade_volume; }
            set { _average_trade_volume = ReduceText(value); }
        }

        public double pe { get; set; }
        public double high_price { get; set; }
        public double share_price_change { get; set; }

        private string _market_cap;
        public string market_cap 
        {
            get { return _market_cap; }
            set { _market_cap = ReduceText(value); }
        }

        public string symbol { get; set; }
        public double share_price_change_percent { get; set; }
        public string name { get; set; }
        public string yield { get; set; }
        public string share_price_direction { get; set; }
    }
}
