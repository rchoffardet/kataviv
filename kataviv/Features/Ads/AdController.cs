using kataviv.Features.Weathers;
using Microsoft.AspNetCore.Mvc;

namespace kataviv.Features.Ads;

[ApiController]
[Route("ads")]
public class AdController : Controller
{
    private readonly AdRepository repository;
    private readonly WeatherService weatherService;

    public AdController(AdRepository repository, WeatherService weatherService)
    {
        this.repository = repository;
        this.weatherService = weatherService;
    }

    [HttpPost]
    [Route("")]
    public IActionResult Create([FromBody] AdIn payload)
    {
        var ad = payload.ToAd();
        var id = repository.Create(ad);
        return StatusCode(201, id);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var ad = repository.GetPublished(id);

        if (ad is null)
        {
            return NotFound();
        }

        var weather = await weatherService.Get(ad.location);

        return Ok(AdOut.FromAdAndWeather(ad, weather));
    }

    [HttpPut]
    [Route("{id:guid}/status")]
    public IActionResult Get(Guid id, [FromBody] AdStatus status)
    {
        repository.ChangeStatus(id, status);
        return NoContent();
    }
}