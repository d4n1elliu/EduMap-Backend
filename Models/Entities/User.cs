using System.ComponentModel.DataAnnotations;

namespace EduMap.Models.Entities;

public class User
{
    public int Id { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public string? ProfileImageURL { get; set; }
    public required DateTime CreationDate { get; set; }
}

