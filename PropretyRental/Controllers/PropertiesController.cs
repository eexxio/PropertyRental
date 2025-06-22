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
public class PropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PropertiesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProperties(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? city = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? bedrooms = null,
        [FromQuery] bool? isAvailable = null,
        [FromQuery] string sortBy = "DateListed",
        [FromQuery] string sortOrder = "desc")
    {
        try
        {
            var query = _context.Properties
                .Include(p => p.Owner)
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.PricePerMonth >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PricePerMonth <= maxPrice.Value);
            }

            if (bedrooms.HasValue)
            {
                query = query.Where(p => p.Bedrooms == bedrooms.Value);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(p => p.IsAvailable == isAvailable.Value);
            }

            query = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(p => p.PricePerMonth) 
                    : query.OrderByDescending(p => p.PricePerMonth),
                "city" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(p => p.City) 
                    : query.OrderByDescending(p => p.City),
                "bedrooms" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(p => p.Bedrooms) 
                    : query.OrderByDescending(p => p.Bedrooms),
                _ => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(p => p.DateListed) 
                    : query.OrderByDescending(p => p.DateListed)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var properties = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Address = p.Address,
                    City = p.City,
                    ZipCode = p.ZipCode,
                    PricePerMonth = p.PricePerMonth,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    SquareFootage = p.SquareFootage,
                    IsAvailable = p.IsAvailable,
                    DateListed = p.DateListed,
                    DateRented = p.DateRented,
                    OwnerName = $"{p.Owner.FirstName} {p.Owner.LastName}"
                })
                .ToListAsync();

            return Ok(new
            {
                Properties = properties,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving properties", Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProperty(int id)
    {
        try
        {
            var property = await _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.Id == id)
                .Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Address = p.Address,
                    City = p.City,
                    ZipCode = p.ZipCode,
                    PricePerMonth = p.PricePerMonth,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    SquareFootage = p.SquareFootage,
                    IsAvailable = p.IsAvailable,
                    DateListed = p.DateListed,
                    DateRented = p.DateRented,
                    OwnerName = $"{p.Owner.FirstName} {p.Owner.LastName}"
                })
                .FirstOrDefaultAsync();

            if (property == null)
            {
                return NotFound(new { Message = "Property not found" });
            }

            return Ok(property);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while retrieving the property", Error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
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

            var property = new Property
            {
                Title = createPropertyDto.Title,
                Description = createPropertyDto.Description,
                Address = createPropertyDto.Address,
                City = createPropertyDto.City,
                ZipCode = createPropertyDto.ZipCode,
                PricePerMonth = createPropertyDto.PricePerMonth,
                Bedrooms = createPropertyDto.Bedrooms,
                Bathrooms = createPropertyDto.Bathrooms,
                SquareFootage = createPropertyDto.SquareFootage,
                IsAvailable = createPropertyDto.IsAvailable,
                OwnerId = userId,
                DateListed = DateTime.UtcNow
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            var createdProperty = await _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.Id == property.Id)
                .Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Address = p.Address,
                    City = p.City,
                    ZipCode = p.ZipCode,
                    PricePerMonth = p.PricePerMonth,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    SquareFootage = p.SquareFootage,
                    IsAvailable = p.IsAvailable,
                    DateListed = p.DateListed,
                    DateRented = p.DateRented,
                    OwnerName = $"{p.Owner.FirstName} {p.Owner.LastName}"
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, createdProperty);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while creating the property", Error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(int id, [FromBody] CreatePropertyDto updatePropertyDto)
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

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound(new { Message = "Property not found" });
            }

            if (property.OwnerId != userId)
            {
                return Forbid("You can only update your own properties");
            }

            property.Title = updatePropertyDto.Title;
            property.Description = updatePropertyDto.Description;
            property.Address = updatePropertyDto.Address;
            property.City = updatePropertyDto.City;
            property.ZipCode = updatePropertyDto.ZipCode;
            property.PricePerMonth = updatePropertyDto.PricePerMonth;
            property.Bedrooms = updatePropertyDto.Bedrooms;
            property.Bathrooms = updatePropertyDto.Bathrooms;
            property.SquareFootage = updatePropertyDto.SquareFootage;
            property.IsAvailable = updatePropertyDto.IsAvailable;

            await _context.SaveChangesAsync();

            var updatedProperty = await _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.Id == property.Id)
                .Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Address = p.Address,
                    City = p.City,
                    ZipCode = p.ZipCode,
                    PricePerMonth = p.PricePerMonth,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    SquareFootage = p.SquareFootage,
                    IsAvailable = p.IsAvailable,
                    DateListed = p.DateListed,
                    DateRented = p.DateRented,
                    OwnerName = $"{p.Owner.FirstName} {p.Owner.LastName}"
                })
                .FirstAsync();

            return Ok(updatedProperty);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while updating the property", Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound(new { Message = "Property not found" });
            }

            if (property.OwnerId != userId)
            {
                return Forbid("You can only delete your own properties");
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Property deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while deleting the property", Error = ex.Message });
        }
    }
}