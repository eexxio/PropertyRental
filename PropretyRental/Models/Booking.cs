using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropretyRental.Models;

public class Booking
{
    public int Id { get; set; }
    
    [Required]
    public int PropertyId { get; set; }
    
    [Required]
    public string TenantId { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [Range(1, int.MaxValue)]
    public int TotalGuests { get; set; } = 1;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    
    public virtual Property Property { get; set; } = null!;
    
    public virtual ApplicationUser Tenant { get; set; } = null!;
}