using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_02.Models;
using System.Collections.Concurrent;

namespace Assignment_A1_02.Services
{
    public class OpenWeatherService
    {
        ConcurrentDictionary<(string,string), Forecast> _Cached1 = new ConcurrentDictionary<(string,string), Forecast>();
        ConcurrentDictionary<(string,double,double), Forecast> _Cached2 = new ConcurrentDictionary<(string, double, double), Forecast>();

        public EventHandler<string> WeatherForecastAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "9516011d36968c6458853eec14a51b3f"; // Your API Key

        //part of your event code here
        public async Task<Forecast> GetForecastAsync(string City)
        {
            Forecast forecast = null;
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var city = City;
            var key = (date, city);
            if(!_Cached1.TryGetValue(key, out forecast))
             {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

                forecast = await ReadWebApiAsync(uri);
                _Cached1[key] = forecast;
                OnWeatherForecastAvailable($"New weather forecast for {City} available");

               }
            else
            {
                //part of your event code here
                //this method will notify all the subscriber 
                OnWeatherForecastAvailable($"Cached weather forecast for {City} available");
            }
                

            return forecast;

        }

        //this method is resposnsible to raise the event 
        protected virtual void OnWeatherForecastAvailable(string s)
        {
            //checks if there is any subscriber to this event
            WeatherForecastAvailable?.Invoke(this, s);
        }

       public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            Forecast forecast = null;
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var lat = latitude;
            var lon = longitude;
            var key = (date, lat, lon);
            if(_Cached2.TryGetValue(key, out forecast ))
            {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

                forecast = await ReadWebApiAsync(uri);
                _Cached2[key] = forecast;
                //part of your event code here
                OnWeatherForecastAvailable($"New weather forecast for({latitude},{longitude}) available");
            }
            else
            {
                OnWeatherForecastAvailable($"Cached weather forecast for({latitude},{longitude}) available");
            }



            return forecast;
        }

        
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            // part of your read web api code here

            // part of your data transformation to Forecast here

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            //Your Code to convert WeatherApiData to Forecast using Linq.

            Forecast forecast = new Forecast();

            forecast.City = wd.city.name;


            forecast.Items = new List<ForecastItem>();

            wd.list.ForEach(wdListItem => { forecast.Items.Add(GetForecastItem(wdListItem)); });
            return forecast;
        }

        private ForecastItem GetForecastItem(List wdListItem)
        {

            ForecastItem item = new ForecastItem();
            item.DateTime = UnixTimeStampToDateTime(wdListItem.dt);

            item.Temperature = wdListItem.main.temp;
            item.Description = wdListItem.weather.Count > 0 ? wdListItem.weather.First().description : "No info";
            item.WindSpeed = wdListItem.wind.speed;

            return item;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
