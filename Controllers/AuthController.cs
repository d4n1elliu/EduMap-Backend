using Microsoft.AspNetCore.Mvc;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using EduMap.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EduMap.Controllers;

// Marks this class for APi controller with automatic HTTP 400 responses for invalid models
[ApiController]

// Set as the basic route for all the endpoints within this controller to /api/auth
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // Register a new user in the system 
    // JWT token upon successful registration
    // Returns JWT token when registration is successful
    // Returns error message when registration fails
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterUserAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<AuthResponse?>(result.Message, null));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }

    // Authenticates an existing user and returns a JWT token
    // User login credentials (username/email and password)
    // JWT token upon successful authentication
    // Returns JWT token when login is successful
    // Returns error message when login fails
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginUserAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<AuthResponse?>(result.Message, null));

        return Ok(new ApiResponse<AuthResponse?>(result.Message, result.responseData));
    }

   
    // Generates a new JWT token for an already authenticated user (token refresh)
    // Requires a valid JWT token in the Authorization header
    // New JWT token with extended expiration
    // Returns new JWT token
    // Returns error when user claim is invalid
    // Returns when no valid token is provided
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

