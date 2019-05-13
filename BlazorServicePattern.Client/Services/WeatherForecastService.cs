using BlazorServicePattern.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorServicePattern.Client.Services
{
    /// <summary>
    /// Weather Forecast service using WEB API - HttpClient comes from dependency injection
    /// </summary>
    public class WeatherForecastService : IWeatherForecastService
    {

        private HttpClient _http;

        public WeatherForecastService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<WeatherForecast>> WeatherForecasts()
        {
            return await _http.GetJsonAsync<WeatherForecast[]>("api/SampleData/WeatherForecasts");
        }
    }
}
