using System.ComponentModel.DataAnnotations;

namespace PropretyRental.DTOs;

public class CreatePropertyDto
{
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
    
    [Range(0, double.MaxValue)]
    public decimal PricePerNight { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Bedrooms { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Bathrooms { get; set; }
    
    [Range(0, int.MaxValue)]
    public int SquareFootage { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    [Range(1, int.MaxValue)]
    public int MaxGuests { get; set; } = 2;
    
    [Range(1, 365)]
    public int MinStayNights { get; set; } = 1;
    
    [Range(1, 365)]
    public int MaxStayNights { get; set; } = 30;
    
    public TimeSpan CheckInTime { get; set; } = new TimeSpan(15, 0, 0);
    
    public TimeSpan CheckOutTime { get; set; } = new TimeSpan(11, 0, 0);
    
    public bool HasWifi { get; set; } = false;
    
    public bool HasParking { get; set; } = false;
    
    public bool HasKitchen { get; set; } = false;
    
    public bool HasWasher { get; set; } = false;
    
    public bool HasAirConditioning { get; set; } = false;
}