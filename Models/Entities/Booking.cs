using System.ComponentModel.DataAnnotations;

namespace EduMap.Models.Entities;

public class Booking
{
    public int Id { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required int UserId { get; set; }
    public required DateTime CreationDate { get; set; }
}