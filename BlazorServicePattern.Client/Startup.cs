using BlazorServicePattern.Client.Services;
using BlazorServicePattern.Shared;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServicePattern.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // object an IWeatherForecastService using the service listed in _Services_
            services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
