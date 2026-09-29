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

    private const string MissingMentorDetailsMessage =
        "Mentors must provide an About section and a location (longitude and latitude).";

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    // Registers a user (plus a mentor profile for mentors) and returns a JWT
    public async Task<(bool Success, string Message, AuthResponse? responseData)>
        RegisterUserAsync(RegisterRequest request)
    {
        // Store and compare emails in lowercase so lookups are case-insensitive
        string email = request.Email.Trim().ToLowerInvariant();

        #region InputValidation
        // Email taken: allow adding a mentor profile, otherwise reject
        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            bool mentorProfileExists = await _context.MentorProfiles.AnyAsync(m => m.UserId == existingUser.Id);
            if (!mentorProfileExists && request.Role == Role.Mentor)
            {
                if (IsMissingMentorDetails(request))
                    return (false, MissingMentorDetailsMessage, null);

                MentorProfile newMentor = new MentorProfile
                {
                    User = existingUser,
                    About = request.About,
                    Longitude = (float)request.Longitude,
                    Latitude = (float)request.Latitude,
                };
                _context.MentorProfiles.Add(newMentor);
                await _context.SaveChangesAsync();

                return (true, "Created mentor profile for existing user", null);
            }
            else
            {
                return (false, "Email is already registered.", null);
            }
        }

        if (!IsValidEmail(email))
            return (false, "Email is invalid.", null);

        // Password rules
        if (request.Password.Length < 8)
            return (false, "Password must be at least 8 characters", null);

        if (!request.Password.Any(char.IsUpper))
            return (false, "Password must contain a capital letter", null);

        if (!request.Password.Any(char.IsDigit))
            return (false, "Password must contain a number", null);

        if (!request.Password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Password must contain a special character", null);

        if (request.Role == Role.Mentor && IsMissingMentorDetails(request))
            return (false, MissingMentorDetailsMessage, null);
        #endregion

        User newUser = new User
        {
            Email = email,
            PasswordHash = HashPassword(request.Password),
            CreationDate = DateTime.UtcNow,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role
        };

        _context.Users.Add(newUser);

        if (newUser.Role == Role.Mentor)
        {
            MentorProfile newMentor = new MentorProfile
            {
                User = newUser,
                About = request.About,
                Longitude = (float)request.Longitude,
                Latitude = (float)request.Latitude,
            };
            _context.MentorProfiles.Add(newMentor);
        }

        await _context.SaveChangesAsync();

        // Log the user in straight away
        AuthResponse responseData = new AuthResponse
        {
            JwtToken = GenerateJwtToken(newUser)
        };

        return (true, "Successfully registered.", responseData);
    }

    // Logs in with email and password and returns a JWT
    public async Task<(bool Success, string Message, AuthResponse? responseData)> LoginUserAsync(LoginRequest request)
    {
        string email = request.Username.Trim().ToLowerInvariant();

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return (false, "Invalid username/email or password.", null);

        AuthResponse responseData = new AuthResponse
        {
            JwtToken = GenerateJwtToken(user)
        };

        // Sanity-check the new token
        var isValid = ValidateToken(responseData.JwtToken);

        if (!isValid)
        {
            Console.WriteLine($"Generated token failed validation for user {user.Id}");
            return (false, "Token generation failed. Please try again.", null);
        }

        return (true, "Successfully logged in.", responseData);
    }

    // Issues a fresh JWT for an existing user (token refresh)
    public async Task<(bool Success, string Message, Models.Responses.AuthResponse? responseData)> TokenLoginUserAsync(int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        // User may have been deleted since the token was issued
        if (user == null)
            return (false, "Please relogin", null);

        AuthResponse authResponse = new AuthResponse
        {
            JwtToken = GenerateJwtToken(user)
        };
        return (true, "Successfully logged in.", authResponse);
    }

    // True when a mentor request is missing About, Longitude or Latitude
    private static bool IsMissingMentorDetails(RegisterRequest request)
    {
        return string.IsNullOrWhiteSpace(request.About)
            || request.Longitude == null
            || request.Latitude == null;
    }

    // Checks a JWT's signature, expiry, issuer and audience
    private bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Environment.GetEnvironmentVariable("Jwt_SecretKey");

            if (string.IsNullOrEmpty(secretKey))
            {
                Console.WriteLine("Jwt_SecretKey environment variable is missing");
                return false;
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
                ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            // Throws if the token is invalid
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            Console.WriteLine($"Token validation successful for user: {principal.FindFirst(ClaimTypes.NameIdentifier)?.Value}");
            return true;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during token validation: {ex.Message}");
            return false;
        }
    }
    // Basic "x@y.z" email check
    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // Letters, digits, dots and underscores only
    public static bool IsValidUsername(string username)
    {
        string pattern = @"^[a-zA-Z0-9._]+$";
        return Regex.IsMatch(username, pattern);
    }

    // Creates a JWT with the user ID claim, valid for 1 day
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

    // BCrypt password hash
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // True if the password matches the stored hash
    public static bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}