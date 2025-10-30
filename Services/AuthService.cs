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

    // Registers a new user to the system
    // Returns success status, message and authentication response
    public async Task<(bool Success, string Message, AuthResponse? responseData)>
        RegisterUserAsync(RegisterRequest request)
    {
        #region InputValidation
        // Checks if the email is already registered
        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            bool mentorProfileExists = await _context.MentorProfiles.AnyAsync(m => m.UserId == existingUser.Id);
            if (!mentorProfileExists && request.Role == Role.Mentor)
            {
                // Ignore warnings because those variables should always be included in a register request for mentors
                MentorProfile newMentor = new MentorProfile
                {
                    User = existingUser,
                    About = request.About,
                    Longitude = (float)request.Longitude,
                    Latitude = (float)request.Latitude,
                };
                _context.MentorProfiles.Add(newMentor);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return (true, "Created mentor profile for existing user", null);
            }
            else
            {
                return (false, "Email is already registered.", null);
            }
        }

        // Validate email format using regex
        if (!IsValidEmail(request.Email))
            return (false, "Email is invalid.", null);

        // Password validation rules
        if (request.Password.Length < 8)
            return (false, "Password must be at least 8 characters", null);

        if (!request.Password.Any(char.IsUpper))
            return (false, "Password must contain a capital letter", null);

        if (!request.Password.Any(char.IsDigit))
            return (false, "Password must contain a number", null);

        if (!request.Password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Password must contain a special character", null);
        #endregion

        // Create the user object with all required properties
        User newUser = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreationDate = DateTime.UtcNow,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role
        };

        // Add the user object to the context
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

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Generate JWT token for immediate authentication after registration
        AuthResponse responseData = new AuthResponse
        {
            JwtToken = GenerateJwtToken(newUser)
        };

        // If registration is successful, frontend will receive the jwt token
        return (true, "Successfully registered.", responseData);
    }

    // Authenticates a user with username/email and password
    // Returns success status, message and authentication response with JWT token 
    public async Task<(bool Success, string Message, AuthResponse? responseData)> LoginUserAsync(LoginRequest request)
    {
        // Look for the username that matches with the login username
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Username);

        // Check if the username exists and password is correct
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return (false, "Invalid username/email or password.", null);

        // Generate JWT token for authenticated session
        AuthResponse responseData = new AuthResponse
        {
            JwtToken = GenerateJwtToken(user)
        };

        // Validate the generated token to ensure it's properly formed
        var isValid = ValidateToken(responseData.JwtToken);
        Console.WriteLine(responseData.JwtToken);

        if (!isValid)
        {
            // Log the validation failure
            Console.WriteLine($"Generated token failed validation for user {user.Id}");
            return (false, "Token generation failed. Please try again.", null);
        }

        // If login is successful, frontend will receive the jwt token
        return (true, "Successfully logged in.", responseData);
    }

    // Generate a new JWT token for an exisitng user session (token refresh scenario)
    public async Task<(bool Success, string Message, Models.Responses.AuthResponse? responseData)> TokenLoginUserAsync(int userId)
    {
        // Look for the user that matches with the userId
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Look for the userId that matches with the login userId

        // Check if the user still exists in the database
        if (user == null)
            return (false, "Please relogin", null);

        // Generate new JWT token
        AuthResponse authResponse = new AuthResponse
        {
            JwtToken = GenerateJwtToken(user)
        };
        return (true, "Successfully logged in.", authResponse);
    }

    // Validates a JWT token's signature, expiration and claims
    private bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Environment.GetEnvironmentVariable("Jwt_SecretKey");

            // Check if secret key is configured
            if (string.IsNullOrEmpty(secretKey))
            {
                Console.WriteLine("Jwt_SecretKey environment variable is missing");
                return false;
            }

            // Configure token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
                ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero // No tolerance for immediate validation
            };

            // This will throw an exception if the token is invalid
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
    // Validates email format using regular expression
    // Returns true if email format is valid
    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // Validates username format (alphanumeric, dots, and underscores only)
    // Returns true if username format is valid
    public static bool IsValidUsername(string username)
    {
        string pattern = @"^[a-zA-Z0-9._]+$";
        return Regex.IsMatch(username, pattern);
    }

    // Generates a JWT token for a user with user ID claim
    // Return JWT token string
    private string GenerateJwtToken(User user)
    {
        // Create claims for the token - currently only includes user ID
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        // Debug output to check environment variables (remove in production)
        Console.WriteLine(Environment.GetEnvironmentVariable("Jwt_Issuer"));
        Console.WriteLine(Environment.GetEnvironmentVariable("Jwt_Audience"));
        Console.WriteLine(Environment.GetEnvironmentVariable("Jwt_SecretKey"));

        // Create signing credentials with secret key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_SecretKey")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create JWT token with 1-day expiration
        var token = new JwtSecurityToken(
            Environment.GetEnvironmentVariable("Jwt_Issuer"),
            Environment.GetEnvironmentVariable("Jwt_Audience"),
            claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Hashes a password using BCrypt algorithm
    // Return hashed password
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verifies a plain text password against a stored hash
    // returns true if password matches the hash
    public static bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}

