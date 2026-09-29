using Microsoft.EntityFrameworkCore;
using EduMap.Data;
using EduMap.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dotenv.net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EduMap;

public class Program
{
    public static void Main(string[] args)
    {
        // Config comes from dotnet user-secrets, not a .env file
        //DotEnv.Load();

        var builder = WebApplication.CreateBuilder(args);

        // Allow the frontend dev server and the deployed site
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy.WithOrigins(["http://localhost:5173","edumap-gxf4bpfyg3ghbpgy.australiacentral-01.azurewebsites.net"])
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
        });

        // Local dev uses 5046. In production, Azure sets the port through ASPNETCORE_HTTP_PORTS
        if (builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseUrls("http://localhost:5046");
        }

        // Services
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddControllers();

        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<BuddySystemService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
});

        var app = builder.Build();

        app.UseCors("AllowFrontend");
        app.UseRouting();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(); // /swagger
        }

        // Validates the Bearer token and sets context.User
        app.Use(async (context, next) =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var validationParams = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
                        ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_SecretKey"))),
                        ValidateLifetime = true
                    };

                    var principal = handler.ValidateToken(token, validationParams, out _);
                    context.User = principal;
                }
                catch
                {
                    // Invalid token: carry on as anonymous
                }
            }
            await next();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        
        app.MapControllers();

        // Serve the SPA for any unmatched route
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}