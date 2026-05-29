using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Poster { get; set; }
    public string? Backdrop { get; set; }
    public double? Rating { get; set; }
    public int ReleaseYear { get; set; }
    public int? Runtime { get; set; }
    public string? Director { get; set; }
    public List<string> Genres { get; set; } = new List<string>();
    public List<string> Cast { get; set; } = new List<string>();
    public List<string> StreamingProviders { get; set; } = new List<string>();
    public decimal? ImdbRating { get; set; }
    public string? ImdbId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; }
    public decimal AverageUserRating { get; set; }
    public int TotalReviews { get; set; }

    // Navigation properties
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
}
