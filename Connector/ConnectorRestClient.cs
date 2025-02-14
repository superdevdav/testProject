using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using TestHQ;

namespace Connector
{
    public class ConnectorRestClient : ITestConnector
    {
        public ConnectorRestClient() { }

        #region Rest

        /// <summary>
        /// Получение трейдов
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        /// <param name="maxCount">Максимальное количество трейдов</param>
        /// <returns></returns>
        public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            var options = new RestClientOptions($"https://api-pub.bitfinex.com/v2/trades/t{pair}/hist?limit={maxCount}&sort=-1");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            var response = await client.GetAsync(request);
            var tradesArray = JsonSerializer.Deserialize<List<List<JsonElement>>>(response.Content);

            IEnumerable<Trade> result = tradesArray.Select(t => new Trade
            {
                Id = t[0].GetRawText(),
                Time = DateTimeOffset.FromUnixTimeMilliseconds(t[1].GetInt64()),
                Amount = t[2].GetDecimal(),
                Price = t[3].GetDecimal(),
                Side = t[2].GetDecimal() > 0 ? "buy" : "sell",
                Pair = pair
            }).ToList();

            return result;
        }

        /// <summary>
        /// Получение свечей
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        /// <param name="periodInSec">Таймфрейм в секундах</param>
        /// <param name="from">Начальная дата</param>
        /// <param name="to">Конечная дата</param>
        /// <param name="count">Количество свечей</param>
        /// <returns></returns>
        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(
            string pair,
            int periodInSec,
            DateTimeOffset? from,
            DateTimeOffset? to = null,
            long? count = 0
            )
        {
            var period = $"{periodInSec / 60}m";

            var options = new RestClientOptions($"https://api-pub.bitfinex.com/v2/candles/trade%3A{period}%3At{pair}/hist");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            var response = await client.GetAsync(request);
            var candlesArray = JsonSerializer.Deserialize<List<List<JsonElement>>>(response.Content);

            IEnumerable<Candle> result = candlesArray.Select(c => new Candle
            {
                Pair = pair,
                OpenPrice = c[1].GetDecimal(),
                ClosePrice = c[2].GetDecimal(),
                HighPrice = c[3].GetDecimal(),
                LowPrice = c[4].GetDecimal(),
                TotalVolume = c[5].GetDecimal(),
                TotalPrice = c[2].GetDecimal() * c[5].GetDecimal(),
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(c[0].GetInt64())
            }).ToList();

            return result;
        }

        /// <summary>
        /// Получение информации о тикере
        /// </summary>
        /// <param name="pair">Валютная пара</param>
        /// <returns></returns>
        public async Task<Ticker> GetTickerAsync(string pair)
        {
            var options = new RestClientOptions($"https://api-pub.bitfinex.com/v2/ticker/t{pair}");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            var response = await client.GetAsync(request);
            
            var tickerJson =  JsonSerializer.Deserialize<JsonElement>(response.Content);
            
            var ticker = new Ticker
            {
                Pair = pair,
                Bid = tickerJson[0].GetDecimal(),
                BidSize = tickerJson[1].GetDecimal(),
                Ask = tickerJson[2].GetDecimal(),
                AskSize = tickerJson[3].GetDecimal(),
                DailyChange = tickerJson[4].GetDecimal(),
                DailyChangeRelative = tickerJson[5].GetDecimal(),
                LastPrice = tickerJson[6].GetDecimal(),
                Volume = tickerJson[7].GetDecimal(),
                High = tickerJson[8].GetDecimal(),
                Low = tickerJson[9].GetDecimal()
            };

            return ticker;
        }

        #endregion

        #region Socket

        public event Action<Trade> NewBuyTrade
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public event Action<Trade> NewSellTrade
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public event Action<Candle> CandleSeriesProcessing
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public void SubscribeTrades(string pair, int maxCount = 100)
        {
        }

        public void UnsubscribeTrades(string pair)
        {
        }

        public void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
        }

        public void UnsubscribeCandles(string pair)
        {
        }

        #endregion
    }
}
