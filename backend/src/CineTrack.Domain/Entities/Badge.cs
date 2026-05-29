using System;

namespace CineTrack.Domain.Entities;

public class Badge
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
}
