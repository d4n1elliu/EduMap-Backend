using System.ComponentModel.DataAnnotations;

namespace EduMap.Models.Entities;

public class User
{
    public int Id { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PasswordHash { get; set; }
    public string? ProfileImageURL { get; set; }
    public required DateTime CreationDate { get; set; }
    public required Role Role { get; set; } = Role.Student;
    public Course? Course { get; set; }
}

