using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropretyRental.Data;
using PropretyRental.DTOs;
using PropretyRental.Models;
using System.Security.Claims;

namespace PropretyRental.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createReviewDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var booking = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Property.Owner)
                .FirstOrDefaultAsync(b => b.Id == createReviewDto.BookingId);

            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found" });
            }

            if (booking.TenantId != userId)
            {
                return Forbid("You can only review bookings where you were the guest");
            }

            if (booking.Status != "Approved")
            {
                return BadRequest(new { Message = "You can only review approved bookings" });
            }

            if (booking.EndDate > DateTime.Today)
            {
                return BadRequest(new { Message = "You can only review completed stays" });
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookingId == createReviewDto.BookingId);

            if (existingReview != null)
            {
                return BadRequest(new { Message = "You have already reviewed this booking" });
            }

            var review = new Review
            {
                BookingId = createReviewDto.BookingId,
                GuestId = userId,
                HostId = booking.Property.OwnerId,
                StarRating = createReviewDto.StarRating,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var createdReview = await _context.Reviews
                .Include(r => r.Guest)
                .Include(r => r.Host)
                .Include(r => r.Booking)
                .Include(r => r.Booking.Property)
                .Where(r => r.Id == review.Id)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    GuestId = r.GuestId,
                    GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    HostId = r.HostId,
                    HostName = $"{r.Host.FirstName} {r.Host.LastName}",
                    StarRating = r.StarRating,
                    CreatedAt = r.CreatedAt,
                    PropertyTitle = r.Booking.Property.Title
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, createdReview);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while creating the review", Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        try
        {
            var review = await _context.Reviews
                .Include(r => r.Guest)
                .Include(r => r.Host)
                .Include(r => r.Booking)
                .Include(r => r.Booking.Property)
                .Where(r => r.Id == id)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    GuestId = r.GuestId,
                    GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    HostId = r.HostId,
                    HostName = $"{r.Host.FirstName} {r.Host.LastName}",
                    StarRating = r.StarRating,
                    CreatedAt = r.CreatedAt,
                    PropertyTitle = r.Booking.Property.Title
                })
                .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound(new { Message = "Review not found" });
            }

            return Ok(review);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving the review", Error = ex.Message });
        }
    }

    [HttpGet("host/{hostId}")]
    public async Task<IActionResult> GetHostReviews(string hostId)
    {
        try
        {
            var reviews = await _context.Reviews
                .Include(r => r.Guest)
                .Include(r => r.Host)
                .Include(r => r.Booking)
                .Include(r => r.Booking.Property)
                .Where(r => r.HostId == hostId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    GuestId = r.GuestId,
                    GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    HostId = r.HostId,
                    HostName = $"{r.Host.FirstName} {r.Host.LastName}",
                    StarRating = r.StarRating,
                    CreatedAt = r.CreatedAt,
                    PropertyTitle = r.Booking.Property.Title
                })
                .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.StarRating) : 0;

            return Ok(new
            {
                Reviews = reviews,
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = reviews.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving host reviews", Error = ex.Message });
        }
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetBookingReview(int bookingId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var booking = await _context.Bookings
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found" });
            }

            if (booking.TenantId != userId && booking.Property.OwnerId != userId)
            {
                return Forbid("You can only view reviews for your own bookings or properties");
            }

            var review = await _context.Reviews
                .Include(r => r.Guest)
                .Include(r => r.Host)
                .Include(r => r.Booking)
                .Include(r => r.Booking.Property)
                .Where(r => r.BookingId == bookingId)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    GuestId = r.GuestId,
                    GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    HostId = r.HostId,
                    HostName = $"{r.Host.FirstName} {r.Host.LastName}",
                    StarRating = r.StarRating,
                    CreatedAt = r.CreatedAt,
                    PropertyTitle = r.Booking.Property.Title
                })
                .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound(new { Message = "No review found for this booking" });
            }

            return Ok(review);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving the booking review", Error = ex.Message });
        }
    }
}