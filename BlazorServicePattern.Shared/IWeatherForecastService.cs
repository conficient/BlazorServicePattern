using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServicePattern.Shared
{
    /// <summary>
    /// Service interface for our WeatherForecast service
    /// </summary>
    public interface IWeatherForecastService
    {
        /// <summary>
        /// Get weather forecast data
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<WeatherForecast>> WeatherForecasts();
    }
}
