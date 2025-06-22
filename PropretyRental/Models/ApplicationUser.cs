using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PropretyRental.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}