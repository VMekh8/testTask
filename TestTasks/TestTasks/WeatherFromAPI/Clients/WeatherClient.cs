using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestTasks.WeatherFromAPI.Models;

namespace TestTasks.WeatherFromAPI.Services
{
    public class WeatherClient
    {
        private const string BaseUrl = "http://api.openweathermap.org";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = !string.IsNullOrWhiteSpace(apiKey) ? apiKey : throw new ArgumentException("API key cannot be null or empty.");
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<WeatherResponse> GetDataAsync(float lat, float lon, int days)
        {
            if (days < 1 || days > 5)
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be between 1 and 5.");

            var requestUri = $"/data/2.5/forecast?lat={lat}&lon={lon}&cnt={8 * days}&appid={_apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherResponse>(jsonResponse)
                       ?? throw new JsonException("Failed to deserialize weather response.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HTTP request error: {e.Message}");
                throw;
            }
            catch (JsonException e)
            {
                Console.WriteLine("Error parsing JSON response.");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
                throw;
            }
        }
    }
}
