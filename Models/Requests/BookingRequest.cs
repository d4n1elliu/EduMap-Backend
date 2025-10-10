using System.ComponentModel.DataAnnotations;

namespace EduMap.Models.Requests;

public class BookingRequest
{
    public required DateTime StartTime { get; set; }
    public required TimeSpan Duration { get; set; }
    public required int MentorId { get; set; }

}   