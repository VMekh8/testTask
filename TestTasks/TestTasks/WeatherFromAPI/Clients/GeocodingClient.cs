using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using TestTasks.WeatherFromAPI.Models;

namespace TestTasks.WeatherFromAPI.Clients
{
    public class GeocodingClient
    {
        private const string BaseUrl = "http://api.openweathermap.org/";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeocodingClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<GeocodingResponse> GetAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City must be provided.", nameof(city));

            string requestUri = $"geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<GeocodingResponse>>(jsonResponse);

                return result?.Count > 0 ? result[0] : null;
            }
            catch (HttpRequestException e) when (e.StatusCode != null)
            {
                Console.WriteLine("HTTP Error: " + e.StatusCode);
                throw;
            }
            catch (JsonException e)
            {
                Console.WriteLine("JSON Error: " + e.Message);
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error");
                return null;
            }
        }
    }
}