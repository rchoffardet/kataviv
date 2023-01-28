using System.Collections.ObjectModel;

namespace kataviv.Features.Ads;

public class AdEfRepository : AdRepository
{
    private readonly AdDbContext db;

    public AdEfRepository(AdDbContext db)
    {
        this.db = db;
    }

    public Guid Create(Ad ad)
    {
        var dto = AdDto.FromAd(ad);
        db.ads.Add(dto);
        db.SaveChanges();

        return dto.id;
    }

    public Collection<Ad> GetAll()
    {
        return db.ads.Select(x => x.ToAd()).ToCollection();
    }

    public Ad? GetPublished(Guid id)
    {
        return db.ads
            .FirstOrDefault(x => x.status == (byte)AdStatus.Published && x.id == id)
            ?.ToAd();
    }

    public void ChangeStatus(Guid id, AdStatus status)
    {
        var identifyingDto = new AdDto { id = id };
        db.ads.Attach(identifyingDto);
        identifyingDto.status = (byte)status;
        db.SaveChanges();
    }

    public void Seed(ICollection<Ad> seeds)
    {
        foreach (var seed in seeds)
        {
            Create(seed);
        }
    }
}

internal static class LinqExtension
{
    public static Collection<T> ToCollection<T>(this IEnumerable<T> items)
    {
        return new Collection<T>(items.ToList());
    }
}