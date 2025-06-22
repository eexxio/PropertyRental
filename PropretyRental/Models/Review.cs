using System.ComponentModel.DataAnnotations;

namespace PropretyRental.Models;

public class Review
{
    public int Id { get; set; }
    
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    public string GuestId { get; set; } = string.Empty;
    
    [Required]
    public string HostId { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int StarRating { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Booking Booking { get; set; } = null!;
    
    public virtual ApplicationUser Guest { get; set; } = null!;
    
    public virtual ApplicationUser Host { get; set; } = null!;
}