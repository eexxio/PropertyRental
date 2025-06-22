using Microsoft.AspNetCore.Identity;
using PropretyRental.Models;

namespace PropretyRental.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!context.Users.Any())
        {
            var testUser = new ApplicationUser
            {
                UserName = "test@example.com",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St, Test City",
                DateRegistered = DateTime.UtcNow,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(testUser, "TestPass123!");

            var testProperties = new List<Property>
            {
                new Property
                {
                    Title = "Beautiful Downtown Apartment",
                    Description = "A stunning 2-bedroom apartment in the heart of downtown with modern amenities and city views.",
                    Address = "456 Downtown Ave",
                    City = "Test City",
                    ZipCode = "12345",
                    PricePerMonth = 1500.00m,
                    Bedrooms = 2,
                    Bathrooms = 2,
                    SquareFootage = 1200,
                    IsAvailable = true,
                    OwnerId = testUser.Id,
                    DateListed = DateTime.UtcNow.AddDays(-10)
                },
                new Property
                {
                    Title = "Cozy Suburban House",
                    Description = "A charming 3-bedroom house in a quiet suburban neighborhood, perfect for families.",
                    Address = "789 Suburban Rd",
                    City = "Suburb Town",
                    ZipCode = "67890",
                    PricePerMonth = 2200.00m,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    SquareFootage = 1800,
                    IsAvailable = true,
                    OwnerId = testUser.Id,
                    DateListed = DateTime.UtcNow.AddDays(-5)
                },
                new Property
                {
                    Title = "Modern Studio Loft",
                    Description = "A sleek studio apartment with high ceilings and industrial design elements.",
                    Address = "321 Loft St",
                    City = "Art District",
                    ZipCode = "54321",
                    PricePerMonth = 1200.00m,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    SquareFootage = 800,
                    IsAvailable = false,
                    OwnerId = testUser.Id,
                    DateListed = DateTime.UtcNow.AddDays(-15),
                    DateRented = DateTime.UtcNow.AddDays(-3)
                }
            };

            context.Properties.AddRange(testProperties);
            await context.SaveChangesAsync();
        }
    }
}