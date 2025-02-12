using Connector;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConnectorTests
{
    public class RestClientTests
    {
        [Fact]
        public async Task GetNewTradesAsync_ShouldReturnTrades()
        {
            var client = new ConnectorRestClient();
            var trades = await client.GetNewTradesAsync("BTCUSD", 5);

            Assert.NotNull(trades);
            Assert.True(trades.Any());
        }

        [Fact]
        public async Task GetCandleSeriesAsync_ShouldReturnCandles()
        {
            var client = new ConnectorRestClient();
            var candles = await client.GetCandleSeriesAsync("BTCUSD", 60, null, null, null);

            Assert.NotNull(candles);
            Assert.True(candles.Any());
        }

        [Fact]
        public async Task GetTickerAsync()
        {
            var client = new ConnectorRestClient();
            var ticker = await client.GetTickerAsync("BTCUSD");

            Assert.NotNull(ticker);
            Assert.Equal("BTCUSD", ticker.Pair);
        }
    }
}