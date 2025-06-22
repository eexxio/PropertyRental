using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Models;

namespace PropertyRental.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PricePerNight).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Owner)
                  .WithMany(u => u.Properties)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.PricePerNight);
            entity.HasIndex(e => e.IsAvailable);
            entity.HasIndex(e => e.DateListed);
            entity.HasIndex(e => e.MaxGuests);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasMany(e => e.Properties)
                  .WithOne(p => p.Owner)
                  .HasForeignKey(p => p.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasMany(e => e.Bookings)
                  .WithOne(b => b.Tenant)
                  .HasForeignKey(b => b.TenantId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasMany(e => e.ReceivedReviews)
                  .WithOne(r => r.Host)
                  .HasForeignKey(r => r.HostId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Property)
                  .WithMany(p => p.Bookings)
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Tenant)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Booking)
                  .WithMany()
                  .HasForeignKey(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Guest)
                  .WithMany()
                  .HasForeignKey(e => e.GuestId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Host)
                  .WithMany()
                  .HasForeignKey(e => e.HostId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BookingId).IsUnique();
            entity.HasIndex(e => e.GuestId);
            entity.HasIndex(e => e.HostId);
            entity.HasIndex(e => e.StarRating);
        });
    }
}