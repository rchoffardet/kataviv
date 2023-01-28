using kataviv.Features.Weathers;

namespace kataviv.Features.Ads;

public record AdOut(
    string title,
    string description,
    Coordinates location,
    Adkind type,
    Weather? weather
)
{
    public static AdOut FromAdAndWeather(Ad ad, Weather? weather)
    {
        return new(
            ad.title,
            ad.description,
            ad.location,
            ad.kind,
            weather
        );
    }
}