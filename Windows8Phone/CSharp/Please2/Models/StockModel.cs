using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Please2.Models
{
    public class StockModel
    {
        public string opening_price { get; set; }
        public string low_price { get; set; }
        public double share_price { get; set; }
        public string stock_exchange { get; set; }
        public string trade_volume { get; set; }
        public string average_trade_volume { get; set; }
        public double pe { get; set; }
        public double high_price { get; set; }
        public double share_price_change { get; set; }
        public string market_cap { get; set; }
        public string symbol { get; set; }
        public double share_price_change_percent { get; set; }
        public string name { get; set; }
        public string yield { get; set; }
        public string share_price_direction { get; set; }
    }
}
