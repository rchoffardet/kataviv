using kataviv.Features.Ads;

namespace kataviv.Features.Weathers;

public interface WeatherService
{
    Task<Weather?> Get(Coordinates coords);
}