namespace PropretyRental.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string GuestId { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string HostId { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
}