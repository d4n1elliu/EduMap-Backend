using EduMap.Models.Entities;

namespace EduMap.Models.Responses;

public class MentorResponse
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ProfileEmoji { get; set; }
    public required DateTime CreationDate { get; set; }
    public required Role Role { get; set; } = Role.Mentor;
    public string? Course { get; set; }
    public string? Gender { get; set; }
    public required float Longitude { get; set;  }
    public required float Latitude { get; set;  }
    // public required List<Skill> Skills { get; set; } = new List<Skill>();
}

