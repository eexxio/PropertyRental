using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.DTOs;
using PropertyRental.Models;
using System.Security.Claims;

namespace PropertyRental.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
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

            if (createBookingDto.StartDate >= createBookingDto.EndDate)
            {
                return BadRequest(new { Message = "End date must be after start date" });
            }

            if (createBookingDto.StartDate < DateTime.Today)
            {
                return BadRequest(new { Message = "Start date cannot be in the past" });
            }

            var stayNights = (createBookingDto.EndDate - createBookingDto.StartDate).Days;
            if (stayNights < 1)
            {
                return BadRequest(new { Message = "Stay must be at least 1 night" });
            }

            var property = await _context.Properties.FindAsync(createBookingDto.PropertyId);
            if (property == null)
            {
                return NotFound(new { Message = "Property not found" });
            }

            if (!property.IsAvailable)
            {
                return BadRequest(new { Message = "Property is not available for booking" });
            }

            if (createBookingDto.TotalGuests > property.MaxGuests)
            {
                return BadRequest(new { Message = $"Property can accommodate maximum {property.MaxGuests} guests" });
            }

            if (stayNights < property.MinStayNights)
            {
                return BadRequest(new { Message = $"Minimum stay is {property.MinStayNights} nights" });
            }

            if (stayNights > property.MaxStayNights)
            {
                return BadRequest(new { Message = $"Maximum stay is {property.MaxStayNights} nights" });
            }

            var conflictingBooking = await _context.Bookings
                .Where(b => b.PropertyId == createBookingDto.PropertyId && 
                           b.Status == "Approved" &&
                           ((b.StartDate <= createBookingDto.EndDate && b.EndDate >= createBookingDto.StartDate)))
                .FirstOrDefaultAsync();

            if (conflictingBooking != null)
            {
                return BadRequest(new { Message = "Property is already booked for the selected dates" });
            }

            var totalPrice = property.PricePerNight * stayNights;

            var booking = new Booking
            {
                PropertyId = createBookingDto.PropertyId,
                TenantId = userId,
                StartDate = createBookingDto.StartDate,
                EndDate = createBookingDto.EndDate,
                Notes = createBookingDto.Notes,
                TotalGuests = createBookingDto.TotalGuests,
                TotalPrice = totalPrice,
                Status = "Pending",
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var createdBooking = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .Where(b => b.Id == booking.Id)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    PropertyId = b.PropertyId,
                    PropertyTitle = b.Property.Title,
                    PropertyAddress = b.Property.Address,
                    TenantId = b.TenantId,
                    TenantName = $"{b.Tenant.FirstName} {b.Tenant.LastName}",
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status,
                    BookingDate = b.BookingDate,
                    Notes = b.Notes,
                    TotalGuests = b.TotalGuests,
                    TotalPrice = b.TotalPrice
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, createdBooking);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while creating the booking", Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
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
                .Include(b => b.Tenant)
                .Where(b => b.Id == id && (b.TenantId == userId || b.Property.OwnerId == userId))
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    PropertyId = b.PropertyId,
                    PropertyTitle = b.Property.Title,
                    PropertyAddress = b.Property.Address,
                    TenantId = b.TenantId,
                    TenantName = $"{b.Tenant.FirstName} {b.Tenant.LastName}",
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status,
                    BookingDate = b.BookingDate,
                    Notes = b.Notes,
                    TotalGuests = b.TotalGuests,
                    TotalPrice = b.TotalPrice
                })
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found" });
            }

            return Ok(booking);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving the booking", Error = ex.Message });
        }
    }

    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetUserBookings()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var bookings = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .Where(b => b.TenantId == userId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    PropertyId = b.PropertyId,
                    PropertyTitle = b.Property.Title,
                    PropertyAddress = b.Property.Address,
                    TenantId = b.TenantId,
                    TenantName = $"{b.Tenant.FirstName} {b.Tenant.LastName}",
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status,
                    BookingDate = b.BookingDate,
                    Notes = b.Notes,
                    TotalGuests = b.TotalGuests,
                    TotalPrice = b.TotalPrice
                })
                .ToListAsync();

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving bookings", Error = ex.Message });
        }
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetPropertyBookings(int propertyId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null)
            {
                return NotFound(new { Message = "Property not found" });
            }

            if (property.OwnerId != userId)
            {
                return Forbid("You can only view bookings for your own properties");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .Where(b => b.PropertyId == propertyId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    PropertyId = b.PropertyId,
                    PropertyTitle = b.Property.Title,
                    PropertyAddress = b.Property.Address,
                    TenantId = b.TenantId,
                    TenantName = $"{b.Tenant.FirstName} {b.Tenant.LastName}",
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = b.Status,
                    BookingDate = b.BookingDate,
                    Notes = b.Notes,
                    TotalGuests = b.TotalGuests,
                    TotalPrice = b.TotalPrice
                })
                .ToListAsync();

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving property bookings", Error = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
    {
        try
        {
            if (string.IsNullOrEmpty(status) || !new[] { "Approved", "Rejected" }.Contains(status))
            {
                return BadRequest(new { Message = "Status must be 'Approved' or 'Rejected'" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var booking = await _context.Bookings
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found" });
            }

            if (booking.Property.OwnerId != userId)
            {
                return Forbid("You can only update bookings for your own properties");
            }

            if (booking.Status != "Pending")
            {
                return BadRequest(new { Message = "Only pending bookings can be updated" });
            }

            booking.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Booking {status.ToLower()} successfully", BookingId = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while updating the booking", Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found" });
            }

            if (booking.TenantId != userId)
            {
                return Forbid("You can only cancel your own bookings");
            }

            if (booking.Status == "Approved" && booking.StartDate <= DateTime.Today)
            {
                return BadRequest(new { Message = "Cannot cancel approved bookings that have already started" });
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Booking cancelled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while cancelling the booking", Error = ex.Message });
        }
    }
}