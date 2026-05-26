using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities
{
    /// <summary>
    /// Represents a user account in CineTrack
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        
        // Profile data
        public List<string> FavoriteGenres { get; set; } = new();
        public bool IsPublic { get; set; } = true;
        
        // Auth tokens
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        
        // OAuth
        public string? GoogleId { get; set; }
        public string? AppleId { get; set; }
        
        // Status
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        
        // Relationships
        public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Badge> Badges { get; set; } = new List<Badge>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}