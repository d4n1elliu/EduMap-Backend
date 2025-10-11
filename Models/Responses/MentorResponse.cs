using System.ComponentModel.DataAnnotations;
using EduMap.Models.Entities;

namespace EduMap.Models.Responses;

public class MentorResponse
{
    public int Id { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ProfileImageURL { get; set; }
    public required DateTime CreationDate { get; set; }
    public required Role Role { get; set; } = Role.Mentor;
    public Course? Course { get; set; }
}

