using System;

namespace CineTrack.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public int Rating { get; set; } // 1-10
    public string? Comment { get; set; }
    public List<string>? Tags { get; set; } = new List<string>();
    public bool HasWatched { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Likes { get; set; }
    public int Dislikes { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Movie? Movie { get; set; }
}
