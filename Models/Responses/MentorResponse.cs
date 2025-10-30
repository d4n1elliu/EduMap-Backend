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
    public string? ProfileEmoji { get; set; }
    public required DateTime CreationDate { get; set; }
    public required Role Role { get; set; } = Role.Mentor;
    public string? Course { get; set; }
    public required float Longitude { get; set;  }
    public required float Latitude { get; set;  }
    // public required List<Skill> Skills { get; set; } = new List<Skill>();
}

