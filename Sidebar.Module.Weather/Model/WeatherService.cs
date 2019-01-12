using System;
using System.Threading.Tasks;
using Sidebar.Common;

namespace Sidebar.Module.Weather.Model
{
    public static class WeatherService
    {
        public static async Task<WeatherInfo> GetWeatherInfo(string location, string language)
        {
            string apiKey = ConfigurationHelper.GetAppSetting("OpenWeatherApiKey");
            string requestUrl = String.Format(UrlFormat, location, language, apiKey);

            WeatherData weatherData = await HttpService.GetSerializedObject<WeatherData>(requestUrl);
            if (weatherData == null)
                return new WeatherInfo();
            else
                return new WeatherInfo(weatherData);
        }

        private const string UrlFormat = "http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&type=accurate&units=metric&cnt=3&lang={1}&appid={2}";
    }
}
