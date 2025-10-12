using System.ComponentModel.DataAnnotations;

namespace EduMap.Models.Entities;

public class Booking
{
    public int Id { get; set; }
    public required DateTime StartTime { get; set; }
    public required TimeSpan Duration { get; set; }
    public required User User { get; set; }
    public required int UserId { get; set; }
    public required User Mentor { get; set; }
    public required int MentorId { get; set; }
    public required bool IsConfirmed { get; set; }
}
