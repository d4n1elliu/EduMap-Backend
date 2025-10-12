using EduMap.Data;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using Microsoft.EntityFrameworkCore;
using EduMap.Models.Entities;

namespace EduMap.Services;

public class BuddySystemServices
{
    private readonly AppDbContext _context;

    public BuddySystemServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> BookMentorsAsync(BookingRequest request, int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Look for the user that matches the userId
        User? mentor = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.MentorId); // Look for the mentor that matches the requested mentorId
        if (mentor == null)
            return (false, "Mentor doesn't exist!");
        if (user == null)
            return (false, "User doesn't exist!");

        if (user.Role != Role.Student)
            return (false, "You must be a student to book a mentor");

        // Create the booking object
        Booking booking = new Booking
        {
            User = user,
            UserId = userId,
            StartTime = request.StartTime,
            Duration = request.Duration,
            Mentor = mentor,
            MentorId = request.MentorId,
            IsConfirmed = false
        };

        _context.Bookings.Add(booking); // Add the user object to the context
        await _context.SaveChangesAsync(); // Save changes to the database

        return (true, "Success");
    }

    public async Task<(bool Success, string Message, List<BookingResponse> Bookings)> GetMentorBookingsAsync(int userId)
    {
        List<Booking> bookings = await _context.Bookings.Include(b => b.User).Include(b => b.Mentor).Where(b => b.UserId == userId || b.MentorId == userId).ToListAsync();
        Console.WriteLine(bookings.Count);

        List<BookingResponse> bookingsResponse = bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            FirstName = b.Mentor.FirstName,
            LastName = b.Mentor.LastName,
            StartTime = b.StartTime,
            IsConfimed = b.IsConfirmed,
            Course = b.User.Course
        }).ToList();


        return (true, "Success", bookingsResponse);
    }

    public async Task<(bool Success, string Message, List<MentorResponse> Mentors)> GetMentorsAsync()
    {
        List<User> mentors = await _context.Users.Where(u => u.Role == Role.Mentor).ToListAsync();

        List<MentorResponse> mentorsResponse = mentors.Select(u => new MentorResponse
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            ProfileImageURL = u.ProfileImageURL,
            CreationDate = u.CreationDate,
            Role = u.Role,
            Course = u.Course
        }).ToList();

        return (true, "Success", mentorsResponse);
    }

    public async Task<(bool Success, string Message)> ConfirmBookingAsync(int userId, int bookingId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Look for the username that matches with the userId

        if (user == null)
            return (false, "User not found");

        if (user.Role != Role.Student)
            return (false, "You must be a mentor to accept the booking");

        Booking? booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId); // Look for the booking that matches with the bookingId

        return (true, "Booking confirmed");
    }
}
