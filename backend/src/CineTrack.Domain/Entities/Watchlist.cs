using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities
{
    /// <summary>
    /// Represents a user's watchlist
    /// </summary>
    public class Watchlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WatchlistType Type { get; set; }
        public bool IsPublic { get; set; }
        
        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Relationships
        public User? User { get; set; }
        public ICollection<WatchlistItem> Items { get; set; } = new List<WatchlistItem>();
    }

    public enum WatchlistType
    {
        ToWatch,
        Watched,
        Favorites,
        Custom
    }
}