using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kataviv.Features.Ads;

[Table("ads")]
public class AdDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid id { get; set; }

    public string title { get; set; }

    public string description { get; set; }

    public double latitude { get; set; }
    public double longitude { get; set; }

    public byte status { get; set; }

    public byte kind { get; set; }

    public static AdDto FromAd(Ad ad)
    {
        return new AdDto
        {
            id = ad.id,
            title = ad.title,
            description = ad.description,
            latitude = ad.location.latitude,
            longitude = ad.location.longitude,
            status = (byte)ad.status,
            kind = (byte)ad.kind,
        };
    }

    public Ad ToAd()
    {
        return new Ad(
            id,
            title,
            description,
            new Coordinates(latitude, longitude),
            (Adkind)kind,
            (AdStatus)status
        );
    }
}