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

// Routes: /api/buddySystem/...
[ApiController]
[Route("api/[controller]")]
public class BuddySystemController : ControllerBase
{
    private readonly BuddySystemService _buddySystemService;

    public BuddySystemController(BuddySystemService buddySystemService)
    {
        _buddySystemService = buddySystemService;
    }

    // Student books a session with a mentor
    [Authorize]
    [HttpPost("create-booking")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _buddySystemService.BookMentorsAsync(request, int.Parse(userIdClaim));

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message));
    }

    // Gets the user's bookings, as either student or mentor
    [Authorize]
    [HttpGet("get-bookings")]
    public async Task<IActionResult> GetBookings()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _buddySystemService.GetMentorBookingsAsync(int.Parse(userIdClaim));

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message, result.Bookings));
    }

    // Mentor accepts a booking. Body is the raw booking ID
    [Authorize]
    [HttpPost("confirm-booking")]
    public async Task<IActionResult> ConfirmBookings([FromBody] int bookingId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _buddySystemService.ConfirmBookingAsync(int.Parse(userIdClaim), bookingId);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message));
    }

    // Saves a map event and returns all of the user's events
    [Authorize]
    [HttpPost("save-event")]
    public async Task<IActionResult> SaveEvent(EventsRequest _event)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _buddySystemService.SaveEvent(_event, int.Parse(userIdClaim));
        return Ok(new ApiResponse<object>(result.Message, result.Events));
    }

    // Gets the user's map events
    [Authorize]
    [HttpGet("get-events")]
    public async Task<IActionResult> GetEvents()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _buddySystemService.GetEvents(int.Parse(userIdClaim));
        return Ok(new ApiResponse<object>(result.Message, result.Events));
    }

    // Lists all mentors (no login needed)
    [HttpGet("get-mentors")]
    public async Task<IActionResult> GetMentors()
    {
        var result = await _buddySystemService.GetMentorsAsync();

        if (!result.Success)
        return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message, result.Mentors));
    }

    // Not used in the demo
    [HttpPost("add-mentor")]
    public async Task<IActionResult> CreateMentorProfile(CreateMentorProfileRequest profile)
    {
        var result = await _buddySystemService.CreateMentorProfile(profile);
        return Ok(new ApiResponse<object>(result.Message));
    }
}

