using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropretyRental.Models;

namespace PropretyRental.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PricePerMonth).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Owner)
                  .WithMany(u => u.Properties)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.PricePerMonth);
            entity.HasIndex(e => e.IsAvailable);
            entity.HasIndex(e => e.DateListed);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasMany(e => e.Properties)
                  .WithOne(p => p.Owner)
                  .HasForeignKey(p => p.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}