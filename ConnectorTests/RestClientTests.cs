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
    }
}