using Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHQ;

namespace ConnectorTests
{
    public class WebSocketClientTests
    {
        private readonly ConnectorWebSocketClient _client;

        public WebSocketClientTests()
        {
            _client = new ConnectorWebSocketClient();
        }

        [Fact]
        public async Task SubscribeTrades_ShouldTriggerNewTradeEvent_TradeUpdated()
        {
            Trade receivedTrade = null;
            _client.NewBuyTrade += trade => receivedTrade = trade;

            string subscribeMessage = "{ \"event\": \"subscribed\", \"channel\": \"trades\", \"chanId\": 12345, \"symbol\": \"tBTCUSD\" }";
            await SimulateWebSocketMessage(subscribeMessage);

            string testMessage = "[12345, \"tu\", [987654321000, 1707825600000, 0.1, 50000.5]]";
            await SimulateWebSocketMessage(testMessage);

            Assert.NotNull(receivedTrade);
            Assert.Equal("buy", receivedTrade.Side);
            Assert.Equal(0.1m, receivedTrade.Amount);
            Assert.Equal(50000.5m, receivedTrade.Price);
        }
        
        [Fact]
        public async Task SubscribeTrades_ShouldTriggerNewTradeEvent_TradeExecuted()
        {
            Trade receivedTrade = null;
            _client.NewBuyTrade += trade => receivedTrade = trade;

            string subscribeMessage = "{ \"event\": \"subscribed\", \"channel\": \"trades\", \"chanId\": 12345, \"symbol\": \"tBTCUSD\" }";
            await SimulateWebSocketMessage(subscribeMessage);

            string testMessage = "[12345, \"te\", [987654321000, 1707825600000, 0.1, 50000.5]]";
            await SimulateWebSocketMessage(testMessage);

            Assert.NotNull(receivedTrade);
            Assert.Equal("buy", receivedTrade.Side);
            Assert.Equal(0.1m, receivedTrade.Amount);
            Assert.Equal(50000.5m, receivedTrade.Price);
        }

        [Fact]
        public async Task SubscribeTrades_ShouldTriggerNewTradeEvent_FundingTradeExecuted()
        {
            Trade receivedTrade = null;
            _client.NewBuyTrade += trade => receivedTrade = trade;

            string subscribeMessage = "{ \"event\": \"subscribed\", \"channel\": \"trades\", \"chanId\": 12345, \"symbol\": \"tBTCUSD\" }";
            await SimulateWebSocketMessage(subscribeMessage);

            string testMessage = "[12345, \"fte\", [987654321000, 1707825600000, 0.1, 50000.5]]";
            await SimulateWebSocketMessage(testMessage);

            Assert.NotNull(receivedTrade);
            Assert.Equal("buy", receivedTrade.Side);
            Assert.Equal(0.1m, receivedTrade.Amount);
            Assert.Equal(50000.5m, receivedTrade.Price);
        }

        [Fact]
        public async Task SubscribeTrades_ShouldTriggerNewTradeEvent_FundingTradeUpdated()
        {
            Trade receivedTrade = null;
            _client.NewBuyTrade += trade => receivedTrade = trade;

            string subscribeMessage = "{ \"event\": \"subscribed\", \"channel\": \"trades\", \"chanId\": 12345, \"symbol\": \"tBTCUSD\" }";
            await SimulateWebSocketMessage(subscribeMessage);

            string testMessage = "[12345, \"ftu\", [987654321000, 1707825600000, 0.1, 50000.5]]";
            await SimulateWebSocketMessage(testMessage);

            Assert.NotNull(receivedTrade);
            Assert.Equal("buy", receivedTrade.Side);
            Assert.Equal(0.1m, receivedTrade.Amount);
            Assert.Equal(50000.5m, receivedTrade.Price);
        }

        [Fact]
        public async Task SubscribeCandles_ShouldTriggerCandleEvent()
        {
            Candle receivedCandle = null;
            _client.CandleSeriesProcessing += candle => receivedCandle = candle;

            string subscribeMessage = "{ \"event\": \"subscribed\", \"channel\": \"candles\", \"chanId\": 12345, \"symbol\": \"tBTCUSD\" }";
            await SimulateWebSocketMessage(subscribeMessage);

            string testMessage = "[12345, [1707825600000, 50000, 50100.7, 50250.3, 49900.2, 125.7]]";
            await SimulateWebSocketMessage(testMessage);

            Assert.NotNull(receivedCandle);
            Assert.Equal(50000.0m, receivedCandle.OpenPrice);
            Assert.Equal(50100.7m, receivedCandle.ClosePrice);
            Assert.Equal(50250.3m, receivedCandle.HighPrice);
            Assert.Equal(49900.2m, receivedCandle.LowPrice);
            Assert.Equal(125.7m, receivedCandle.TotalVolume);
        }

        private async Task SimulateWebSocketMessage(string message)
        {
            // Вызываем приватный метод обработки сообщений через рефлексию
            var handleMessageMethod = typeof(ConnectorWebSocketClient).GetMethod("HandleMessage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            handleMessageMethod?.Invoke(_client, new object[] { message });

            // Небольшая задержка, чтобы события успели обработаться
            await Task.Delay(100);
        }
    }
}
