using kataviv.Features.Ads;
using System.Globalization;

namespace kataviv.Features.Weathers;

public class OpenMeteoWeatherService : WeatherService
{
    private readonly IHttpClientFactory factory;
    private const string baseUrl = "https://api.open-meteo.com/v1/forecast";

    public OpenMeteoWeatherService(IHttpClientFactory factory)
    {
        this.factory = factory;
    }

    public async Task<Weather?> Get(Coordinates coords)
    {
        var client = factory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(200);

        try
        {
            var latitude = $"latitude={coords.latitude.ToString(CultureInfo.InvariantCulture)}";
            var longitude = $"longitude={coords.longitude.ToString(CultureInfo.InvariantCulture)}";
            var url = $"{baseUrl}?{latitude}&{longitude}&current_weather=true";

            var result = await client.GetFromJsonAsync<WeatherResultDto>(url);
            return result?.current_weather?.ToWeather();
        }
        catch (Exception)
        {
            return null;
        }
    }
}