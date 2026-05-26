using System;

namespace CineTrack.Domain.Entities
{
    /// <summary>
    /// Represents a follow relationship between users
    /// </summary>
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Relationships
        public User? Follower { get; set; }
        public User? Following { get; set; }
    }
}