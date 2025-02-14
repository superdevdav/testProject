using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class CryptoPriceFetcher
{
    private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://api-pub.bitfinex.com/v2/") };

    public async Task<Dictionary<string, decimal>> GetPricesAsync()
    {
        var symbols = new string[] { "BTCUSD", "XRPUSD", "XMRUSD", "DSHUSD" };
        var prices = new Dictionary<string, decimal>();

        foreach (var symbol in symbols)
        {
            var response = await _httpClient.GetAsync($"ticker/t{symbol}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<JsonElement>>(json);
                prices[symbol] = data[6].GetDecimal();
            }
        }

        return prices;
    }
}