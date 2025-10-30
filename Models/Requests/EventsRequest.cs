using EduMap.Models.Entities;

namespace EduMap.Models.Requests;

public class EventsRequest
{
    public required int MentorId { get; set; }

    public required string FullName { get; set; }
    public required float Latitude { get; set;  }
    public required float Longitude { get; set;  }
}
