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

    // Creates an unconfirmed booking between a student and a mentor
    public async Task<(bool Success, string Message)> BookMentorsAsync(BookingRequest request, int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        User? mentor = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.MentorId);
        if (mentor == null)
            return (false, "Mentor doesn't exist!");
        if (user == null)
            return (false, "User doesn't exist!");
        if (mentor.Role != Role.Mentor)
            return (false, "That user isn't a mentor");
        if (user.Role != Role.Student)
            return (false, "You must be a student to book a mentor");

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

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return (true, "Success");
    }

    // Gets bookings where the user is the student or the mentor
    public async Task<(bool Success, string Message, List<BookingResponse> Bookings)> GetMentorBookingsAsync(int userId)
    {
        List<Booking> bookings = await _context.Bookings.Include(b => b.User).Include(b => b.Mentor).Where(b => b.UserId == userId || b.MentorId == userId).ToListAsync();

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

    // Lists all mentor profiles with their user details
    public async Task<(bool Success, string Message, List<MentorResponse> Mentors)> GetMentorsAsync()
    {
        var mentors = await _context.MentorProfiles.Include(x => x.User).ToListAsync();

        List<MentorResponse> mentorsResponse = mentors.Select(m => new MentorResponse
        {
            Id = m.UserId,
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

    // Adds a mentor profile to an existing user
    public async Task<(bool Success, string Message)> CreateMentorProfile(CreateMentorProfileRequest profile)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == profile.UserId);
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

    // Saves an event, then returns all of the user's events
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

    // Gets the user's events, placed at each mentor's location
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

    // Mentor accepts a student's booking
    public async Task<(bool Success, string Message)> ConfirmBookingAsync(int userId, int bookingId)
    {   
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return (false, "User not found");

        if (user.Role != Role.Mentor)
            return (false, "You must be a mentor to accept the booking");

        Booking? booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return (false, "Booking not found");

        // Only the mentor the booking was made with can confirm it
        if (booking.MentorId != userId)
            return (false, "You can only confirm your own bookings");

        if (booking.IsConfirmed)
            return (false, "Booking is already confirmed");

        booking.IsConfirmed = true;
        await _context.SaveChangesAsync();

        return (true, "Booking confirmed");
    }
}
