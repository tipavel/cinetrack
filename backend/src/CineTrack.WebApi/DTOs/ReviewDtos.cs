namespace CineTrack.WebApi.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public List<string>? Tags { get; set; }
    public bool HasWatched { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public UserProfileDto? User { get; set; }
}

public class CreateReviewDto
{
    public int MovieId { get; set; }
    public int Rating { get; set; } // 1-10
    public string? Comment { get; set; }
    public List<string>? Tags { get; set; }
    public bool HasWatched { get; set; } = true;
}

public class UpdateReviewDto
{
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public List<string>? Tags { get; set; }
}

public class MovieReviewsDto
{
    public int MovieId { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    public int TotalCount { get; set; }
    public decimal AverageRating { get; set; }
}
