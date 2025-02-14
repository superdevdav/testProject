using CryptoPortfolio;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CryptoPortfolio
{
    public class PortfolioViewModel
    {
        public ObservableCollection<PortfolioBalance> Balances { get; set; } = new ObservableCollection<PortfolioBalance>();

        public async Task LoadPortfolio()
        {
            var fetcher = new CryptoPriceFetcher();
            var prices = await fetcher.GetPricesAsync();

            var balances = new List<PortfolioBalance>();

            var portfolio = new Dictionary<string, decimal>
            {
                { "BTC", 1m },
                { "XRP", 15000m },
                { "XMR", 50m },
                { "DASH", 30m }
            };

            var btcToUsd = prices["BTCUSD"];
            var xrpToUsd = prices["XRPUSD"];
            var xmrToUsd = prices["XMRUSD"];
            var dashToUsd = prices["DSHUSD"];

            /*decimal usdEquivalent =
                    portfolio["XRP"] * xrpToUsd +
                    portfolio["XMR"] * xmrToUsd +
                    portfolio["DASH"] * dashToUsd;

            balances.Add(new PortfolioBalance
            {
                Currency = "USD",
                InUSDT = usdEquivalent,
                InBTC = usdEquivalent / btcToUsd,
                InXRP = usdEquivalent / xrpToUsd,
                InXMR = usdEquivalent / xmrToUsd,
                InDASH = usdEquivalent / dashToUsd
            });*/

            foreach (var currency in portfolio.Keys)
            {
                if (currency == "BTC")
                {
                    decimal btcEquivalent =
                        portfolio["BTC"] +
                        (portfolio["XRP"] * (xrpToUsd / btcToUsd)) +
                        (portfolio["XMR"] * (xmrToUsd / btcToUsd)) +
                        (portfolio["DASH"] * (dashToUsd / btcToUsd));

                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InBTC = btcEquivalent,
                        InXRP = btcToUsd / xrpToUsd,
                        InXMR = btcToUsd / xmrToUsd,
                        InDASH = btcToUsd / dashToUsd,
                        InUSDT = btcToUsd
                    });
                }

                if (currency == "XRP")
                {
                    decimal xrpEquivalent =
                        portfolio["XRP"] +
                        (portfolio["BTC"] * (btcToUsd / xrpToUsd)) +
                        (portfolio["XMR"] * (xmrToUsd / xrpToUsd)) +
                        (portfolio["DASH"] * (dashToUsd / xrpToUsd));

                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InXRP = xrpEquivalent,
                        InBTC = xrpToUsd / btcToUsd,
                        InXMR = xrpToUsd / xmrToUsd,
                        InDASH = xrpToUsd / dashToUsd,
                        InUSDT = xrpToUsd
                    });
                }

                if (currency == "XMR")
                {
                    decimal xmrEquivalent =
                        portfolio["XMR"] +
                        (portfolio["BTC"] * (btcToUsd / xmrToUsd)) +
                        (portfolio["XRP"] * (xrpToUsd / xmrToUsd)) +
                        (portfolio["DASH"] * (dashToUsd / xmrToUsd));

                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InXMR = xmrEquivalent,
                        InBTC = xmrToUsd / btcToUsd,
                        InXRP = xmrToUsd / xrpToUsd,
                        InDASH = xmrToUsd / dashToUsd,
                        InUSDT = xmrToUsd
                    });
                }

                if (currency == "DASH")
                {
                    decimal dashEquivalent =
                        portfolio["DASH"] +
                        (portfolio["BTC"] * (btcToUsd / dashToUsd)) +
                        (portfolio["XRP"] * (xrpToUsd / dashToUsd)) +
                        (portfolio["XMR"] * (xmrToUsd / dashToUsd));

                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InDASH = dashEquivalent,
                        InBTC = dashToUsd / btcToUsd,
                        InXRP = dashToUsd / xrpToUsd,
                        InXMR = dashToUsd / xmrToUsd,
                        InUSDT = dashToUsd
                    });
                }
            }

            Balances.Clear();
            foreach (var balance in balances)
            {
                Balances.Add(balance);
            }
        }
    }
}