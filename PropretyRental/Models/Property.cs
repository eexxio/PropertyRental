using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PropretyRental.Models;

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
    public decimal PricePerMonth { get; set; }
    
    public int Bedrooms { get; set; }
    
    public int Bathrooms { get; set; }
    
    public int SquareFootage { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime DateListed { get; set; } = DateTime.UtcNow;
    
    public DateTime? DateRented { get; set; }
    
    [Required]
    public string OwnerId { get; set; } = string.Empty;
    
    public virtual ApplicationUser Owner { get; set; } = null!;
}