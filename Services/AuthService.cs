using System.Text.RegularExpressions;
using EduMap.Data;
using EduMap.Models.Entities;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EduMap.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message, AuthResponse? responseData)>
        RegisterUserAsync(RegisterRequest request)
    {
        #region InputValidation
        if (await _context.Users.AnyAsync(u => u.Email == request.Email)) // Check if the email is already registered
            return (false, "Email is already registered.", null);

        if (!IsValidEmail(request.Email))
            return (false, "Email is invalid.", null);

        if (request.Password.Length < 8)
            return (false, "Password must be at least 8 characters", null);

        if (!request.Password.Any(char.IsUpper))
            return (false, "Password must contain a capital letter", null);

        if (!request.Password.Any(char.IsDigit))
            return (false, "Password must contain a number", null);

        if (!request.Password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Password must contain a special character", null);
        #endregion

        // Create the user object
        User newUser = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreationDate = DateTime.UtcNow,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role
        };

        _context.Users.Add(newUser); // Add the user object to the context
        await _context.SaveChangesAsync(); // Save changes to the database

        AuthResponse responseData = new AuthResponse
        {
            JwtToken = GenerateJwtToken(newUser)
        };

        // If registration is successful, frontend will receive the jwt token
        return (true, "Successfully registered.", responseData);
    }

    public async Task<(bool Success, string Message, AuthResponse? responseData)> LoginUserAsync(LoginRequest request)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Username); // Look for the username that matches with the login username

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash)) // Check if the username exists and password is correct
            return (false, "Invalid username/email or password.", null);

        AuthResponse responseData = new AuthResponse
        {
            JwtToken = GenerateJwtToken(user)
        };

        // If login is successful, frontend will receive the jwt token
        return (true, "Successfully logged in.", responseData);
    }

    public async Task<(bool Success, string Message, Models.Responses.AuthResponse? responseData)> TokenLoginUserAsync(int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Look for the userId that matches with the login userId

        if (user == null) // Check if the user exists
            return (false, "Please relogin", null);


        AuthResponse authResponse = new AuthResponse
        {
            JwtToken = GenerateJwtToken(user)
        };

        return (true, "Successfully logged in.", authResponse);
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public static bool IsValidUsername(string username)
    {
        string pattern = @"^[a-zA-Z0-9._]+$";
        return Regex.IsMatch(username, pattern);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_SecretKey")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            Environment.GetEnvironmentVariable("Jwt_Issuer"),
            Environment.GetEnvironmentVariable("Jwt_Audience"),
            claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}

