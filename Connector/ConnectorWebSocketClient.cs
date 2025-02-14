using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestHQ;
using WebSocketSharp;

namespace Connector
{
    public class ConnectorWebSocketClient : ITestConnector
    {
        private readonly WebSocket _ws;
        private readonly Dictionary<string, int> _tradeChannels = new();
        private readonly Dictionary<string, int> _candleChannels = new();

        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;
        public event Action<Candle> CandleSeriesProcessing;

        public ConnectorWebSocketClient()
        {
            _ws = new WebSocket("wss://api-pub.bitfinex.com/ws/2");
            _ws.OnMessage += (sender, e) => HandleMessage(e.Data);
        }

        #region Rest

        public Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount) { return null; }

        public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0) { return null; }

        public Task<Ticker> GetTickerAsync(string pair) { return null; }

        #endregion

        #region Socket

        /// <summary>
        /// Обработка входящих сообщений WebSocket
        /// </summary>
        /// <param name="message"></param>
        private void HandleMessage(string message)
        {
            using var doc = JsonDocument.Parse(message);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("event", out var eventProp))
            {
                if (eventProp.GetString() == "subscribed")
                {
                    var channel = root.GetProperty("channel").GetString();
                    var chanId = root.GetProperty("chanId").GetInt32();
                    var pair = root.GetProperty("symbol").GetString();
                    
                    if (channel == "trades")
                        _tradeChannels[pair] = chanId;
                    else if (channel == "candles")
                        _candleChannels[pair] = chanId;
                }
                return;
            }

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 1)
            {
                var secondElement = root[1].ToString();

                if (secondElement == "te" || secondElement == "tu" || secondElement == "fte" || secondElement == "ftu")
                {
                    var tradeId = root[0].GetInt32();

                    var pair = _tradeChannels.FirstOrDefault(x => x.Value == tradeId).Key;

                    var thirdElement = root[2];

                    ProcessTrade(thirdElement, pair);
                }
                else
                {
                    var candleDataArray = root[1];
                    ProcessCandle(candleDataArray);
                }
            }
        }

        /// <summary>
        /// Обработка входящих данных о трейдах
        /// </summary>
        /// <param name="data">JSON-объект с данными о трейде</param>
        /// <param name="pair">Валютная пара</param>
        private void ProcessTrade(JsonElement data, string pair)
        {
            if (data.ValueKind == JsonValueKind.Array)
            {
                var trade = new Trade
                {
                    Id = data[0].ToString(),
                    Time = DateTimeOffset.FromUnixTimeMilliseconds(data[1].GetInt64()),
                    Amount = data[2].GetDecimal(),
                    Price = data[3].GetDecimal(),
                    Side = data[2].GetDecimal() > 0 ? "buy" : "sell",
                    Pair = pair
                };

                if (trade.Side == "buy")
                    NewBuyTrade?.Invoke(trade);
                else
                    NewSellTrade?.Invoke(trade);
            }
        }

        /// <summary>
        /// Обработка входящих данных о свечах
        /// </summary>
        /// <param name="data">JSON-объект с данными о свечах</param>
        private void ProcessCandle(JsonElement data)
        {
            if (data.ValueKind == JsonValueKind.Array)
            {
                var candle = new Candle
                {
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(data[0].GetInt64()),
                    OpenPrice = data[1].GetDecimal(),
                    ClosePrice = data[2].GetDecimal(),
                    HighPrice = data[3].GetDecimal(),
                    LowPrice = data[4].GetDecimal(),
                    TotalVolume = data[5].GetDecimal(),
                    TotalPrice = data[2].GetDecimal() * data[5].GetDecimal()
                };

                CandleSeriesProcessing?.Invoke(candle);
            }
        }

        /// <summary>
        /// Подписка на получение данных о трейдах для указанной валютной пары
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        /// <param name="maxCount">Максимальное количество сообщений</param>
        public void SubscribeTrades(string pair, int maxCount = 100)
        {
            var msg = JsonSerializer.Serialize(new { @event = "subscribe", channel = "trades", symbol = "t" + pair });
            _ws.Send(msg);
        }

        /// <summary>
        /// Отписка от получения данных о трейдах для указанной валютной пары
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        public void UnsubscribeTrades(string pair)
        {
            if (_tradeChannels.TryGetValue(pair, out var chanId))
            {
                var msg = JsonSerializer.Serialize(new { @event = "unsubscribe", chanId });
                _ws.Send(msg);
                _tradeChannels.Remove(pair);
            }
        }

        /// <summary>
        /// Подписка на получение данных о свечах для указанной валютной пары
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        /// <param name="periodInSec">Таймфрейм в секундах</param>
        /// <param name="from">Начальная дата</param>
        /// <param name="to">Конечная дата</param>
        /// <param name="count">Количество свечей</param>
        public void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            string timeframe = periodInSec switch
            {
                60 => "1m",
                300 => "5m",
                900 => "15m",
                3600 => "1h",
                86400 => "1D",
                _ => "1m"
            };

            var key = $"trade:{timeframe}:t{pair}";
            var msg = JsonSerializer.Serialize(new { @event = "subscribe", channel = "candles", key });
            _ws.Send(msg);
        }

        /// <summary>
        /// Отписка от получения данных о свечах для указанной валютной пары
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        public void UnsubscribeCandles(string pair)
        {
            if (_candleChannels.TryGetValue(pair, out var chanId))
            {
                var msg = JsonSerializer.Serialize(new { @event = "unsubscribe", chanId });
                _ws.Send(msg);
                _candleChannels.Remove(pair);
            }
        }

        /// <summary>
        /// Установка соединения с WebSocket-сервером
        /// </summary>
        public void Connect()
        {
            _ws.Connect();
        }

        #endregion
    }
}
