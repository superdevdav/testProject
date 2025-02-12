using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHQ
{
    public class Ticker
    {
        public string Pair { get; set; }
        public decimal LastPrice { get; set; }
        public decimal DailyChange { get; set; }
        public decimal DailyChangePercent { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
    }
}
