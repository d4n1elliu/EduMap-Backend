namespace EduMap.Models.Requests;

public class CreateMentorProfileRequest
{
    public required int UserId { get; set;  }
    public required string About { get; set; }
    public required float Longitude { get; set;  }
    public required float Latitude { get; set;  }
}   