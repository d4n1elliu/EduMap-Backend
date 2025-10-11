using Microsoft.AspNetCore.Mvc;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using EduMap.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

    [HttpPost("create-booking")]
    public async Task<IActionResult> Register([FromBody] BookingRequest request)
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

        return Ok(new ApiResponse<object>(result.Message));
    }

    [Authorize]
    [HttpPost("confirm-booking")]
    public async Task<IActionResult> ConfirmBookings([FromBody] int bookingId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _bookingService.ConfirmBookingAsync(int.Parse(userIdClaim), bookingId);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message));
    }
}

