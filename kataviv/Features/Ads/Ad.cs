namespace kataviv.Features.Ads;

public record Ad(
    Guid id,
    string title,
    string description,
    Coordinates location,
    Adkind kind,
    AdStatus status = AdStatus.Draft
)
{ }