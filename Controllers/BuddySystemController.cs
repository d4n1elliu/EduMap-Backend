using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using EduMap.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace EduMap.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuddySystemController : ControllerBase
{
    private readonly BuddySystemServices _bookingService;

    public BuddySystemController(BuddySystemServices bookingService)
    {
        _bookingService = bookingService;
    }

    [Authorize]
    [HttpPost("create-booking")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _bookingService.BookMentorsAsync(request, int.Parse(userIdClaim));

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message));
    }

    [Authorize]
    [HttpGet("get-bookings")]
    public async Task<IActionResult> GetBookings()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _bookingService.GetMentorBookingsAsync(int.Parse(userIdClaim));

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message, result.Bookings));
    }

    [HttpPost("confirm-booking")]
    public async Task<IActionResult> ConfirmBookings([FromBody] int bookingId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(new ApiResponse<object>("Please relogin"));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _bookingService.ConfirmBookingAsync(int.Parse(userIdClaim), bookingId);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message));
    }

    [HttpGet("get-mentors")]
    public async Task<IActionResult> GetMentors()
    {
        var result = await _bookingService.GetMentorsAsync();

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message, result.Mentors));
    }
}

