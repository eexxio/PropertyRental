using System.ComponentModel.DataAnnotations;

namespace PropertyRental.DTOs;

public class CreateReviewDto
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5")]
    public int StarRating { get; set; }
}