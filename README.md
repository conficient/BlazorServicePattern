# Blazor Service Pattern
Demonstrates a pattern for designing components so they can work as either client or server side

### Introduction

When creating Blazor components or applications, one has a key decision to make about the
[hosting model](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.0)
you are going to be using.

At the time of writing, only the server-side model is going to have a go-live licence at 
some point during, probably July 2019. The client-side WebAssembly model may
be delayed until late 2019 or even 2020. So if you wanted to develop an application 
now, you'd have to use the server-side model initially, even if you plan to
migrate to the client-side model later.

However, if you do so, it's important not to code pages and components in 
your Blazor client to use services which only work in a server-side 
environment.  

For example, assume that the data the `WeatherForecast` service
in the Blazor file-new examples came from a database. The 
`WeatherForecastService.cs` class would be able to run on the server 
but would fail in a client side model.

### Solution

The solution is to use an interface service pattern to abstract the implementation 
of the service away from the component, and have different implementations of this 
pattern - one for the client side app, and another for the server side app.

When we switch from server to client. we just change the dependency injections for the service in the 
application to use the alternative service.

## Weather Forecast v2

Let's use the Weather Forecast in the Blazor samples as an example. In the 
ASP.NET Core hosted Blazor sample, this service is implemented in the 
`SampleDataController.cs` as follows:
```cs
public IEnumerable<WeatherForecast> WeatherForecasts()
{
    var rng = new Random();
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
    });
}
```
On the client page `FetchData.razor` we have the code to retrieve this:
```cs
protected override async Task OnInitAsync()
{
    forecasts = await Http.GetJsonAsync<WeatherForecast[]>("api/SampleData/WeatherForecasts");
}
```
So this page will only work on client-side web-api hosted Blazor. A server-side 
implementation _could_ call the API but it would be unneccessary since it's
already running on the server.

### Interface

First we add a new interface to our Shared project: `IWeatherForecastService` 
with a method to obtain the data. This abstracts the implementation.
```cs
Task<IEnumerable<WeatherForecast>> WeatherForecasts();
```
We're using async methods here since the client API call in Blazor via HttpClient 
have to be async as well.

We then change our `SampleDataController.cs` to be async, and implement this interface:
```cs
public class SampleDataController : Controller, IWeatherForecastService
{
    [HttpGet("[action]")]
    public Task<IEnumerable<WeatherForecast>> WeatherForecasts()
    {
        return Task.Run(() =>
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        });
    }
```
In a real-world example it's likely the service to obtain the data would also be 
async so we wouldn't need `Task.Run`

### Service 
Next, we create a service implementation. I added a `Services` folder to the client 
project, and added `WeatherForecastService.cs`:
```cs
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
```
This service implements our interface using a WebAPI call - it needs the `HttpClient` 
to be injected into the constructor.

Next we amend the `Startup.cs` to set up a dependency injection so the page can use this implementation:
```cs
    services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
```
Then in `FetchData.razor` we change from using `HttpClient` (and hard-coding our web-api URL!)
to using the injected service:
```cs
protected override async Task OnInitAsync()
{
    forecasts = (await forecastService.WeatherForecasts()).ToArray();
}
```
The FetchData page can now be the same for both client-side and server side Blazor.

If changed over server-side-Blazor, we could use the `SampleDataController` as our 
service implementation directly, by adding this to our server's `Startup.cs`:
```cs
    services.AddSingleton<IWeatherForecastService, SampleDataController>();
```

#### Notes

I assumed that it would be a Singleton service but this would depend on 
the type of service.
