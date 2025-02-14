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

            foreach (var currency in portfolio.Keys)
            {
                if (currency == "BTC")
                {
                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InBTC = portfolio[currency],
                        InXRP = btcToUsd / xrpToUsd,
                        InXMR = btcToUsd / xmrToUsd,
                        InDASH = btcToUsd / dashToUsd,
                        InUSDT = portfolio[currency] * btcToUsd
                    });
                }

                if (currency == "XRP")
                {
                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InXRP = portfolio[currency],
                        InBTC = xrpToUsd / btcToUsd,
                        InXMR = xrpToUsd / xmrToUsd,
                        InDASH = xrpToUsd / dashToUsd,
                        InUSDT = portfolio[currency] * xrpToUsd
                    });
                }

                if (currency == "XMR")
                {
                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InXMR = portfolio[currency],
                        InBTC = xmrToUsd / btcToUsd,
                        InXRP = xmrToUsd / xrpToUsd,
                        InDASH = xmrToUsd / dashToUsd,
                        InUSDT = portfolio[currency] * xmrToUsd
                    });
                }

                if (currency == "DASH")
                {
                    balances.Add(new PortfolioBalance
                    {
                        Currency = currency,
                        InDASH = portfolio[currency],
                        InBTC = dashToUsd / btcToUsd,
                        InXRP = dashToUsd / xrpToUsd,
                        InXMR = dashToUsd / xmrToUsd,
                        InUSDT = portfolio[currency] * dashToUsd
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