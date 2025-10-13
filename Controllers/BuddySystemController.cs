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

// Marks this class as API controller with automatic HTTP 400 responses for invalid models
[ApiController]

// Setting the base route for all endpoints in this controller to /api/buddySystem
[Route("api/[controller]")]
public class BuddySystemController : ControllerBase
{
    private readonly BuddySystemServices _bookingService;

    public BuddySystemController(BuddySystemServices bookingService)
    {
        _bookingService = bookingService;
    }
    /* Creates a new booking session with a mentor
        Booking details including mentor ID, start time, and duration
        Success message
        Booking created successfully
        Invalid request or booking failed
        User not authenticated */
        
    [Authorize] // Requires valid JWT token
    [Authorize]
    [HttpPost("create-booking")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        // Extracting user ID from JWT token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Validating user ID claim exists
        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        // Call service to create booking, passing the authenticated user's ID
        var result = await _bookingService.BookMentorsAsync(request, int.Parse(userIdClaim));

        // Return error if booking creation failed
        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        // Return success response
        return Ok(new ApiResponse<object>(result.Message));
    }


    /* Retrieves all bookings for the authenticated user (both as student and as mentor)
        List of booking details
        Returns list of bookings
        Invalid user claim
        User not authenticated*/
    [Authorize]
    [HttpGet("get-bookings")]
    public async Task<IActionResult> GetBookings()
    {
        // Extracting user ID from JWT token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Validating user ID claim exists
        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        // Call service to get bookings for the authenticated user
        var result = await _bookingService.GetMentorBookingsAsync(int.Parse(userIdClaim));

        // Return error if fetching bookings failed
        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        // Return success response with bookings data
        return Ok(new ApiResponse<object>(result.Message, result.Bookings));
    }
   
    /* Confirms a booking request (mentor accepting a student's booking)
    
        ID of the booking to confirm<
        Success message
        Booking confirmed successfully
        Invalid request or confirmation failed
        User not authenticated */
     
    [HttpPost("confirm-booking")]
    public async Task<IActionResult> ConfirmBookings([FromBody] int bookingId)
    {   
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized(new ApiResponse<object>("Please relogin"));
        }

        // Extracting user ID from JWT token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Validate that user ID claim exists
        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        // Call service to confirm the booking
        var result = await _bookingService.ConfirmBookingAsync(int.Parse(userIdClaim), bookingId);

        // Return error if confirmation failed
        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        // Return success response
        return Ok(new ApiResponse<object>(result.Message));
    }
    

    /* Retrieves all available mentors in the system
        List of mentor profiles
        Returns list of mentors
        Error fetching mentors*/

    [HttpGet("get-mentors")]
    public async Task<IActionResult> GetMentors()
    {
        // Call service to get all mentors (no authentication required)
        var result = await _bookingService.GetMentorsAsync();

        // Return error if fetching mentors failed
        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        // Return success response with mentors data
        return Ok(new ApiResponse<object>(result.Message, result.Mentors));
    }
}

