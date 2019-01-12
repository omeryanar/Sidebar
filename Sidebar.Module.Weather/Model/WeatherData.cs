using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sidebar.Module.Weather.Model
{
    [DataContract]
    public class City
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }
    }

    [DataContract]
    public class Weather
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }

    [DataContract]
    public class Temperature
    {
        [DataMember(Name = "day")]
        public double Day { get; set; }

        [DataMember(Name = "min")]
        public double Minimum { get; set; }

        [DataMember(Name = "max")]
        public double Maximum { get; set; }
    }

    [DataContract]
    public class Forecast
    {
        [DataMember(Name = "dt")]
        public long Date { get; set; }

        [DataMember(Name = "clouds")]
        public int Clouds { get; set; }

        [DataMember(Name = "humidity")]
        public int Humidity { get; set; }

        [DataMember(Name = "speed")]
        public double WindSpeed { get; set; }

        [DataMember(Name = "weather")]
        public Weather[] Weather { get; set; }

        [DataMember(Name = "temp")]
        public Temperature Temperature { get; set; }
    }

    [DataContract]
    public class WeatherData
    {
        [DataMember(Name = "city")]
        public City City { get; set; }

        [DataMember(Name = "list")]
        public Forecast[] Forecast { get; set; }
    }

    public class WeatherInfo
    {
        public string Location { get; private set; }

        public List<WeatherInfoDetail> Details { get; private set; }

        public WeatherInfo()
        {
            Location = String.Empty;

            Details = new List<WeatherInfoDetail>();
            for (int i = 0; i < 3; i++)
            {
                Details.Add(new WeatherInfoDetail
                {
                    DayOfWeek = Properties.Resources.Culture.DateTimeFormat.GetDayName(DateTime.Today.AddDays(i).DayOfWeek)
                });
            }
        }

        public WeatherInfo(WeatherData weatherData)
        {
            Location = weatherData.City.Name;
            
            Details = new List<WeatherInfoDetail>();
            foreach (Forecast forecast in weatherData.Forecast)
                Details.Add(new WeatherInfoDetail(forecast));
        }
    }

    public class WeatherInfoDetail
    {
        public int Temperature { get; private set; }

        public int MaxTemperature { get; private set; }

        public int MinTemperature { get; private set; }

        public int WindSpeed { get; private set; }

        public int Clouds { get; private set; }

        public int Humidity { get; private set; }

        public string IconPath { get; private set; }

        public string Condition { get; private set; }

        public string DayOfWeek { get; set; }

        public WeatherInfoDetail()
        {
            Condition = Properties.Resources.NotFound;
            IconPath = String.Format("/Sidebar.Module.Weather;component/Assets/WeatherIcons/{0}{1}.png", 800, "d");
        }

        public WeatherInfoDetail(Forecast forecast)
        {
            Temperature = Convert.ToInt32(Math.Round(forecast.Temperature.Day, 0));
            MaxTemperature = Convert.ToInt32(Math.Round(forecast.Temperature.Maximum, 0));
            MinTemperature = Convert.ToInt32(Math.Round(forecast.Temperature.Minimum, 0));

            WindSpeed = Convert.ToInt32(Math.Round(forecast.WindSpeed, 0));

            Clouds = forecast.Clouds;
            Humidity = forecast.Humidity;

            DateTime dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddSeconds(forecast.Date).ToLocalTime();
            DayOfWeek = Properties.Resources.Culture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);

            Weather weather = forecast.Weather[0];
            Condition = Properties.Resources.Culture.TextInfo.ToTitleCase(weather.Description);
            IconPath = String.Format("/Sidebar.Module.Weather;component/Assets/WeatherIcons/{0}{1}.png", weather.Id, weather.Icon[2]);
        }
    }
}
