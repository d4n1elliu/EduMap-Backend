using EduMap.Data;
using EduMap.Models.Requests;
using Microsoft.EntityFrameworkCore;
using EduMap.Models.Entities;

namespace EduMap.Services;

public class BuddySystenServices
{
    private readonly AppDbContext _context;

    public BuddySystenServices(AppDbContext context)
    {
        _context = context;
    }
    public async Task<(bool Success, string Message)> BookMentorsAsync(BookingRequest request, int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.MentorId); // Look for the username that matches with the login username
        if (user == null)
            return (false, "Error!");

        // Create the user object
        Booking booking = new Booking
        {
            UserId = userId,
            StartTime = request.StartTime,
            Duration = request.Duration,
            MentorId = request.MentorId
        };

        _context.Bookings.Add(booking); // Add the user object to the context
        await _context.SaveChangesAsync(); // Save changes to the database

        return (true, "Success");
    }

    public async Task<(bool Success, string Message)> GetMentorBookingsAsync(int userId)
    {
        List<Booking> bookings = await _context.Bookings.Where(b => b.UserId == userId || b.MentorId == userId)
        return (true, "Success");
    }

    
}