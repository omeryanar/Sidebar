using System;
using System.Net;
using System.Threading.Tasks;
using Sidebar.Common;

namespace Sidebar.Module.Weather.Model
{
    public class WeatherService
    {
        protected const string BaseUrl = "http://api.openweathermap.org/data/2.5/forecast/";

        public static async Task<WeatherInfo> GetWeatherInfo(string location, string language)
        {
            string apiKey = ConfigurationHelper.GetAppSetting("OpenWeatherApiKey");
            string requestUrl = String.Format("daily?q={0}&type=accurate&units=metric&cnt=3&lang={1}&appid={2}", WebUtility.UrlEncode(location), language, apiKey);

            WeatherData weatherData = await HttpClientHelper.GetSerializedObject<WeatherData>(BaseUrl, requestUrl);
            if (weatherData == null)
                return new WeatherInfo();
            else
                return new WeatherInfo(weatherData);
        }
    }
}
