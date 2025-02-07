using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestTasks.CharCounting;
using TestTasks.InternationalTradeTask;
using TestTasks.WeatherFromAPI;
using TestTasks.WeatherFromAPI.Clients;
using TestTasks.WeatherFromAPI.Services;

namespace TestTasks
{
    class Program
    {
        static async Task Main()
        {
            //Below are examples of usage. However, it is not guaranteed that your implementation will be tested on those examples.            
            var stringProcessor = new StringProcessor();
            string str = File.ReadAllText("./CharCounting/StringExample.txt");
            var charCount = stringProcessor.GetCharCount(str, new char[] { 'l', 'r', 'm' });
            charCount.ToList().ForEach(tuple => Console.WriteLine($"{tuple.count} + {tuple.symbol}"));

            var commodityRepository = new CommodityRepository();
            Console.WriteLine(commodityRepository.GetImportTariff("Natural honey"));
            Console.WriteLine(
            commodityRepository.GetExportTariff("Iron/steel scrap not sorted or graded"));

            var weatherManager = new WeatherManager(new GeocodingClient(new HttpClient(), "mysecretkey"), new WeatherClient(new HttpClient(), "mysecretkey"));
            var comparisonResult = await weatherManager.CompareWeather("kyiv,ua", "lviv,ua", 4);
            Console.WriteLine($"{comparisonResult.CityA}\t{comparisonResult.CityB}\t{comparisonResult.RainierDaysCount}, {comparisonResult.WarmerDaysCount}");
        }
    }
}
