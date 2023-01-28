namespace kataviv.Features.Weathers;

public class CurrentWeatherDto
{
    public float temperature { get; set; }
    public byte code { get; set; }
    public float windSpeed { get; set; }
    public float windDirection { get; set; }

    public Weather ToWeather()
    {
        return new Weather(temperature, code, windSpeed, windDirection);
    }
}