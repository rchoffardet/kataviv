using Microsoft.EntityFrameworkCore;

namespace kataviv.Features.Ads;

public class AdDbContext : DbContext
{

    public AdDbContext(DbContextOptions<AdDbContext> options)
        : base(options)
    { }

    public DbSet<AdDto> ads => Set<AdDto>();

    public void Reset()
    {
        ChangeTracker.Clear();
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
}