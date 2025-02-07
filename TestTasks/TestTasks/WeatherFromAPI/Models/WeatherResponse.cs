using System.Collections.Generic;
using Newtonsoft.Json;

public class WeatherResponse
{
    public List<ForecastItem> List { get; set; }
}

public class ForecastItem
{
    public MainData Main { get; set; }
    public List<Weather> Weather { get; set; }
    public RainData Rain { get; set; }
    public string Dt_txt { get; set; }
}

public class MainData
{
    public float Temp { get; set; }
    public int Humidity { get; set; }
}

public class Weather
{
    public string Description { get; set; }
}

public class RainData
{
    [JsonProperty("3h")]
    public float? Volume { get; set; } = 0; 
}
