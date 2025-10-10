using EduMap.Models.Entities;

namespace EduMap.Models.Responses;

public class BookingResponse
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Course? Course { get; set; }
    public required DateTime StartTime { get; set; }
    public required bool IsConfiemd { get; set; }
}
