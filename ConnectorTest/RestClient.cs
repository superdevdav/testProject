﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using TestHQ;

namespace ConnectorTest
{
    public class TestConnector : ITestConnector
    {
        public async Task<IEnumerable<Trade>> GetNewTradesAsync(
            string pair,
            int maxCount
            )
        {
            var options = new RestClientOptions($"https://api-pub.bitfinex.com/v2/trades/t{pair}/hist?limit={maxCount}&sort=-1");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            var response = await client.GetAsync(request);
            return JsonSerializer.Deserialize<IEnumerable<Trade>>(response.Content);
        }

        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(
            string pair, 
            int periodInSec, 
            DateTimeOffset? from, 
            DateTimeOffset? to = null, 
            long? count = 0
            )
        {
            var options = new RestClientOptions($"https://api-pub.bitfinex.com/v2/candles/trade%3A1m%3At{pair}/hist");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            var response = await client.GetAsync(request);
            return JsonSerializer.Deserialize<IEnumerable<Candle>>(response.Content);
        }

        public async Task<Ticker> GetTickerAsync(string pair)
        {
            var options = new RestClientOptions($"https://api-pub.bitfinex.com/v2/ticker/t{pair}");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            var response = await client.GetAsync(request);
            return JsonSerializer.Deserialize<Ticker>(response.Content);
        }

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
    }
}
