namespace CineTrack.WebApi.DTOs;

public class UserRegistrationDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
}

public class UpdateUserProfileDto
{
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
}

public class AuthTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserProfileDto? User { get; set; }
}
