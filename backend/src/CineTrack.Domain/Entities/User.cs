using System;
using System.Collections.Generic;

namespace CineTrack.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }

    // Navigation properties
    public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Badge> Badges { get; set; } = new List<Badge>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<User> Followers { get; set; } = new List<User>();
    public ICollection<User> Following { get; set; } = new List<User>();
}
