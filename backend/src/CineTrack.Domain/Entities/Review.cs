using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities
{
    /// <summary>
    /// Represents a user's review of a movie
    /// </summary>
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public float Rating { get; set; } // 0.5 to 5.0
        public string? ReviewText { get; set; }
        public List<string> Tags { get; set; } = new();
        public DateTime? WatchedDate { get; set; }
        public bool IsSpoiler { get; set; }
        
        // Engagement
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Relationships
        public User? User { get; set; }
        public Movie? Movie { get; set; }
        public ICollection<ReviewComment> Comments { get; set; } = new List<ReviewComment>();
    }
}