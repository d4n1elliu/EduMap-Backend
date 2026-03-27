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
        // We are loading environment varriables from dotnet user-secrets instead of using a .env file. No need for the line below
        //DotEnv.Load();

        var builder = WebApplication.CreateBuilder(args);

        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy.WithOrigins(["http://localhost:5173","edumap-gxf4bpfyg3ghbpgy.australiacentral-01.azurewebsites.net"])
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
        });

        if (builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseUrls("http://localhost:5046");
        }
        else
        {
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(5046); // Bind to 0.0.0.0
            });
        }

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
            app.UseSwaggerUI(); // Optional: UI at /swagger
        }

        // Custom middleware
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
                    context.User = principal; // Set the user manually
                }
                catch
                {
                    // Token validation failed
                }
            }
            await next();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        
        app.MapControllers();

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
