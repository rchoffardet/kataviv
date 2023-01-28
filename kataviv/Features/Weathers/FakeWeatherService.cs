using kataviv.Features.Ads;

namespace kataviv.Features.Weathers;

public class FakeWeatherService : WeatherService
{
    public Task<Weather?> Get(Coordinates coords)
    {
        return Task.FromResult(new Weather(42, 1, 100, 90))!;
    }
}