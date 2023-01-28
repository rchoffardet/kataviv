using System.Collections.ObjectModel;

namespace kataviv.Features.Ads;

public interface AdRepository
{
    Guid Create(Ad ad);
    Collection<Ad> GetAll();
    Ad? GetPublished(Guid id);
    void ChangeStatus(Guid id, AdStatus status);
    void Seed(ICollection<Ad> seeds);
}