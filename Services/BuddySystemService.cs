using EduMap.Data;
using EduMap.Models.Requests;
using EduMap.Models.Responses;
using Microsoft.EntityFrameworkCore;
using EduMap.Models.Entities;

namespace EduMap.Services;

public class BuddySystemService
{
    private readonly AppDbContext _context;

    public BuddySystemService(AppDbContext context)
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

        // Add the user object to the context
        _context.Bookings.Add(booking);

        // Save changes to the database
        await _context.SaveChangesAsync();

        return (true, "Success");
    }

    public async Task<(bool Success, string Message, List<BookingResponse> Bookings)> GetMentorBookingsAsync(int userId)
    {
        // Get all bookings where user is either the student (UserId) or the mentor (MentorId)
        // Include related user and mentor data to avoid lazy loading issues
        List<Booking> bookings = await _context.Bookings.Include(b => b.User).Include(b => b.Mentor).Where(b => b.UserId == userId || b.MentorId == userId).ToListAsync();

        // Transform entity objects to response DTOs
        List<BookingResponse> bookingsResponse = bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            MentorId = b.Mentor.Id,
            FirstName = b.Mentor.FirstName,
            LastName = b.Mentor.LastName,
            StartTime = b.StartTime,
            Duration = b.Duration,
            IsConfimed = b.IsConfirmed,
            Course = b.User.Course
        }).ToList();
        return (true, "Success", bookingsResponse);
    }

    // Retrieves all users with mentor role from the system
    public async Task<(bool Success, string Message, List<MentorResponse> Mentors)> GetMentorsAsync()
    {
        // Get all users with mentor rol
        var mentors = await _context.MentorProfiles.Include(x => x.User).ToListAsync();

        // Transform entity objects to response DTOs
        List<MentorResponse> mentorsResponse = mentors.Select(m => new MentorResponse
        {
            Id = m.Id,
            Email = m.User.Email,
            FirstName = m.User.FirstName,
            LastName = m.User.LastName,
            ProfileEmoji = m.User.ProfileEmoji,
            CreationDate = m.User.CreationDate,
            Role = m.User.Role,
            Course = m.User.Course.ToString(),
            Longitude = m.Longitude,
            Latitude = m.Latitude,
        }).ToList();

        return (true, "Success", mentorsResponse);
    }

    // ✅ CREATE mentor profile entry
    public async Task<(bool Success, string Message)> CreateMentorProfile(CreateMentorProfileRequest profile)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == profile.UserId); // Look for the username that matches with the userId
        if (user == null)
            return (false, "Invalid user iD");

        MentorProfile newMentor = new MentorProfile
        {
            User = user,
            About = profile.About,
            Longitude = profile.Longitude,
            Latitude = profile.Latitude,
        };

        _context.MentorProfiles.Add(newMentor);
        await _context.SaveChangesAsync();

        return (true, "Success");
    }

    public async Task<(bool Success, string Message, List<EventsResponse> Events)> SaveEvent(EventsRequest _event, int userId)
    {
        Event newEvent = new Event
        {
            FullName = _event.FullName,
            StudentId = userId,
            MentorId = _event.MentorId,
            Longitude = _event.Longitude,
            Latitude = _event.Latitude,
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        List<Event> events = await _context.Events.Where(e => e.StudentId == userId || e.MentorId == userId).ToListAsync();
        List<EventsResponse> eventsResponse = events.Select(e =>
        {
            var profile = _context.MentorProfiles
                .Where(m => m.UserId == e.MentorId)
                .Select(m => new { m.Latitude, m.Longitude })
                .FirstOrDefault();

            var emoji = _context.Users
                .Where(u => u.Id == e.MentorId)
                .Select(u => u.ProfileEmoji)
                .FirstOrDefault();

            return new EventsResponse
            {
                Id = e.Id,
                Title = e.FullName,
                Lat = profile?.Latitude ?? 0,
                Lng = profile?.Longitude ?? 0,
                ProfileEmoji = emoji ?? null
            };
        }).ToList();

        return (true, "Success", eventsResponse);
    }

    public async Task<(bool Success, string Message, List<EventsResponse> Events)> GetEvents(int userId)
    {
        List<Event> events = await _context.Events.Where(e => e.StudentId == userId || e.MentorId == userId).ToListAsync();
        List<EventsResponse> eventsResponse = events.Select(e =>
        {
            var profile = _context.MentorProfiles
                .Where(m => m.UserId == e.MentorId)
                .Select(m => new { m.Latitude, m.Longitude })
                .FirstOrDefault();

            var emoji = _context.Users
                .Where(u => u.Id == e.MentorId)
                .Select(u => u.ProfileEmoji)
                .FirstOrDefault();

            return new EventsResponse
            {
                Id = e.Id,
                Title = e.FullName,
                Lat = profile?.Latitude ?? 0,
                Lng = profile?.Longitude ?? 0,
                ProfileEmoji = emoji ?? null
            };
        }).ToList();

        return (true, "Success", eventsResponse);
    }

    // Confirms a booking request (mentor accepting a student's booking)
    public async Task<(bool Success, string Message)> ConfirmBookingAsync(int userId, int bookingId)
    {   
        // Look for the user that matches the userId
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Look for the username that matches with the userId

        if (user == null)
            return (false, "User not found");

        if (user.Role != Role.Student)
            return (false, "You must be a mentor to accept the booking");

        Booking? booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId); // Look for the booking that matches with the bookingId

        return (true, "Booking confirmed");
    }
}
