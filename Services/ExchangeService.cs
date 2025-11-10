using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BudgetTracker.Web.Services
{
    public class ExchangeService
    {
        private readonly HttpClient _httpClient;

        public ExchangeService()
        {
            _httpClient = new HttpClient();
        }

        // ✅ Ottiene il tasso di cambio
        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                var url = $"https://api.frankfurter.app/latest?from={fromCurrency}&to={toCurrency}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var rate = doc.RootElement
                              .GetProperty("rates")
                              .GetProperty(toCurrency.ToUpper())
                              .GetDecimal();

                return rate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel recupero del tasso di cambio: {ex.Message}");
                return 1m;
            }
        }

        // ✅ Nuovo metodo: ottiene la lista di valute disponibili
        public async Task<Dictionary<string, string>> GetSupportedCurrenciesAsync()
        {
            try
            {
                var url = "https://api.frankfurter.app/currencies";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var currencies = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                return currencies ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel recupero delle valute: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }
    }
}
