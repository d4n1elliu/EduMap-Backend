namespace EduMap.Models.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public required int StudentId { get; set; }
        public required int MentorId { get; set; }
        public required string FullName { get; set; }
        public required float Latitude { get; set;  }
        public required float Longitude { get; set;  }
    }
}
