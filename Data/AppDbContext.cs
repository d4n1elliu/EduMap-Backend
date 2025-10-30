using Microsoft.EntityFrameworkCore;
using EduMap.Models.Entities;
using EduMap.Models;

namespace EduMap.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<MentorProfile> MentorProfiles { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MentorProfile>()
            .HasOne(mp => mp.User)
            .WithOne()
            .HasForeignKey<MentorProfile>(mp => mp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

