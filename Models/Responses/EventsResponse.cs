namespace EduMap.Models.Requests;

public class EventsResponse
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    public required float Lat { get; set;  }
    public required float Lng { get; set;  }
    public string? ProfileEmoji { get; set; }
}
