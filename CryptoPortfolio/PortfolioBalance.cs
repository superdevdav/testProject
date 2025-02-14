using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolio
{
    public class PortfolioBalance
    {
        public string Currency { get; set; }
        public decimal InBTC { get; set; }
        public decimal InXRP { get; set; }
        public decimal InXMR { get; set; }
        public decimal InDASH { get; set; }
        public decimal InUSDT { get; set; }
    }
}
