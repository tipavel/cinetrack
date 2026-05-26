using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities
{
    /// <summary>
    /// Represents a movie from TMDb
    /// </summary>
    public class Movie
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public string? BackdropUrl { get; set; }
        public float Rating { get; set; }
        public int VoteCount { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Runtime { get; set; } // in minutes
        public List<string> Genres { get; set; } = new();
        public List<string> Cast { get; set; } = new();
        public string? DirectorName { get; set; }
        public string? Language { get; set; }
        
        // Local ratings
        public float AverageUserRating { get; set; }
        public int UserRatingCount { get; set; }
        
        // Timestamps
        public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
        
        // Relationships
        public ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<StreamingProvider> StreamingProviders { get; set; } = new List<StreamingProvider>();
    }
}