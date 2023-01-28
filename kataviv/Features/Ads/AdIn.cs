using System.ComponentModel.DataAnnotations;

namespace kataviv.Features.Ads;

public class AdIn
{
    [Required]
    public string title { get; set; }

    [Required]
    public string description { get; set; }

    [Required]
    public Coordinates? location { get; set; }

    [Required]
    public Adkind? type { get; set; }

    public Ad ToAd()
    {
        return new Ad(Guid.Empty, title, description, location!, type!.Value);
    }
}