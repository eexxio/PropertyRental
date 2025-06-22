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
    public decimal PricePerMonth { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Bedrooms { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Bathrooms { get; set; }
    
    [Range(0, int.MaxValue)]
    public int SquareFootage { get; set; }
    
    public bool IsAvailable { get; set; } = true;
}