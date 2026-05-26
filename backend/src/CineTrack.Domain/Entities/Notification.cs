using System;

namespace CineTrack.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string? Data { get; set; } // JSON for extra context
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
    }

    public enum NotificationType
    {
        ReleaseDate,
        StreamingAvailable,
        NewFollower,
        ReviewComment,
        ReviewLike,
        ListShared,
        SystemAlert
    }
}