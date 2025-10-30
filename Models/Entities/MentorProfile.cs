namespace EduMap.Models.Entities
{
    public class MentorProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }

        // public int Rating { get; set; } // Create a Rating entity
        // public int Reviews { get; set; } // Create a reviews entity
        public required string About { get; set; }
        public required float Longitude { get; set;  }
        public required float Latitude { get; set;  }
    }
}
