using Microsoft.AspNetCore.Mvc;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using EduMap.Services;

namespace EduMap.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuddySystemController : ControllerBase
{
    private readonly BuddySystenServices _bookingService;

    public BuddySystemController(BuddySystenServices bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("booking")]
    public async Task<IActionResult> Register([FromBody] BookingRequest request)
    {
        var result = await _bookingService.BookMentorsAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(result.Message));

        return Ok(new ApiResponse<object>(result.Message));
    }
}

