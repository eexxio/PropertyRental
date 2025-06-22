using System.ComponentModel.DataAnnotations;

namespace PropretyRental.DTOs;

public class CreateBookingDto
{
    [Required]
    public int PropertyId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [Range(1, int.MaxValue)]
    public int TotalGuests { get; set; } = 1;
}