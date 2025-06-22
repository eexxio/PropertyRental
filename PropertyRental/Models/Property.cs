using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PropertyRental.Models;

public class Property
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string ZipCode { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerNight { get; set; }
    
    public int Bedrooms { get; set; }
    
    public int Bathrooms { get; set; }
    
    public int SquareFootage { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime DateListed { get; set; } = DateTime.UtcNow;
    
    public int MaxGuests { get; set; } = 2;
    
    public int MinStayNights { get; set; } = 1;
    
    public int MaxStayNights { get; set; } = 30;
    
    public TimeSpan CheckInTime { get; set; } = new TimeSpan(15, 0, 0);
    
    public TimeSpan CheckOutTime { get; set; } = new TimeSpan(11, 0, 0);
    
    public bool HasWifi { get; set; } = false;
    
    public bool HasParking { get; set; } = false;
    
    public bool HasKitchen { get; set; } = false;
    
    public bool HasWasher { get; set; } = false;
    
    public bool HasAirConditioning { get; set; } = false;
    
    [Required]
    public string OwnerId { get; set; } = string.Empty;
    
    public virtual ApplicationUser Owner { get; set; } = null!;
    
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}