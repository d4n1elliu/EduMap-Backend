using Microsoft.AspNetCore.Mvc;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using EduMap.Services;

namespace EduMap.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterUserAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<AuthResponse?>(result.Message, null));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginUserAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<AuthResponse?>(result.Message, null));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }
}

