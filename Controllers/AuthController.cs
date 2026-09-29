using Microsoft.AspNetCore.Mvc;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using EduMap.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduMap.Controllers;

// Routes: /api/auth/...
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // Registers a user and returns a JWT
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterUserAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<AuthResponse?>(result.Message, null));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }

    // Logs in with email and password and returns a JWT
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginUserAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<AuthResponse?>(result.Message, null));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }

    // Issues a fresh JWT for a logged-in user
    [Authorize]
    [HttpPost("token")]
    public async Task<IActionResult> Token()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return BadRequest(new ApiResponse<object>("Please relogin"));

        var result = await _authService.TokenLoginUserAsync(int.Parse(userIdClaim));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }

}

