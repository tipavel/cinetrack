using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities;

public class Watchlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int MovieCount { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
