using System;
using System.Linq;
using System.Threading.Tasks;
using TestTasks.WeatherFromAPI.Models;
using TestTasks.WeatherFromAPI.Services;
using System.Collections.Generic;
using TestTasks.WeatherFromAPI.Clients;

namespace TestTasks.WeatherFromAPI
{
    public class WeatherManager
    {
        private readonly GeocodingClient _geoClient;
        private readonly WeatherClient _weatherClient;

        public WeatherManager(GeocodingClient geoClient, WeatherClient weatherClient)
        {
            _geoClient = geoClient ?? throw new ArgumentNullException(nameof(geoClient));
            _weatherClient = weatherClient ?? throw new ArgumentNullException(nameof(weatherClient));
        }

        public async Task<WeatherComparisonResult> CompareWeather(string cityA, string cityB, int dayCount)
        {
            if (dayCount is < 1 or > 5)
                throw new ArgumentException("Кількість днів повинна бути від 1 до 5.");

            var coordinatesCityA = await _geoClient.GetAsync(cityA)
                                   ?? throw new ArgumentException($"Місто '{cityA}' не знайдено.");
            var coordinatesCityB = await _geoClient.GetAsync(cityB)
                                   ?? throw new ArgumentException($"Місто '{cityB}' не знайдено.");

            var forecastCityA = await _weatherClient.GetDataAsync(coordinatesCityA.Lat, coordinatesCityA.Lon, dayCount);
            var forecastCityB = await _weatherClient.GetDataAsync(coordinatesCityB.Lat, coordinatesCityB.Lon, dayCount);

            var warmerDays = CompareMetrics(forecastCityA, forecastCityB,
                item => item.Main.Temp, Enumerable.Average);

            var rainierDays = CompareMetrics(forecastCityA, forecastCityB,
                item => item.Rain?.Volume ?? 0, Enumerable.Sum);

            return new WeatherComparisonResult(cityA, cityB, warmerDays, rainierDays);
        }

        private int CompareMetrics(
            WeatherResponse cityA,
            WeatherResponse cityB,
            Func<ForecastItem, double> selector,
            Func<IEnumerable<double>, double> aggregator)
        {
            var groupedCityA = GroupByDateAndAggregate(cityA, selector, aggregator);
            var groupedCityB = GroupByDateAndAggregate(cityB, selector, aggregator);

            return groupedCityA.Count(day => groupedCityB.TryGetValue(day.Key, out var valueB) && day.Value > valueB);
        }

        private Dictionary<DateTime, double> GroupByDateAndAggregate(
            WeatherResponse city,
            Func<ForecastItem, double> selector,
            Func<IEnumerable<double>, double> aggregator)
        {
            return city?.List?
                .Where(x => x != null)
                .GroupBy(x => DateTime.Parse(x.Dt_txt).Date)
                .ToDictionary(day => day.Key, data => aggregator(data.Select(selector)))
                ?? new Dictionary<DateTime, double>();
        }
    }
}
