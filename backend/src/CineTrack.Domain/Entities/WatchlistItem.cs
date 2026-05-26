using System;

namespace CineTrack.Domain.Entities
{
    /// <summary>
    /// Represents a movie in a watchlist
    /// </summary>
    public class WatchlistItem
    {
        public int Id { get; set; }
        public int WatchlistId { get; set; }
        public int MovieId { get; set; }
        public int Position { get; set; } // For drag-drop ordering
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        
        // Relationships
        public Watchlist? Watchlist { get; set; }
        public Movie? Movie { get; set; }
    }
}