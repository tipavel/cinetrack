namespace CineTrack.WebApi.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Poster { get; set; }
    public string? Backdrop { get; set; }
    public double? Rating { get; set; }
    public int ReleaseYear { get; set; }
    public int? Runtime { get; set; }
    public string? Director { get; set; }
    public List<string> Genres { get; set; } = new List<string>();
    public List<string> Cast { get; set; } = new List<string>();
    public List<string> StreamingProviders { get; set; } = new List<string>();
    public decimal? ImdbRating { get; set; }
    public decimal AverageUserRating { get; set; }
    public int TotalReviews { get; set; }
    public int ViewCount { get; set; }
}

public class CreateMovieDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Poster { get; set; }
    public string? Backdrop { get; set; }
    public double? Rating { get; set; }
    public int ReleaseYear { get; set; }
    public int? Runtime { get; set; }
    public string? Director { get; set; }
    public List<string>? Genres { get; set; } = new List<string>();
    public List<string>? Cast { get; set; } = new List<string>();
    public List<string>? StreamingProviders { get; set; } = new List<string>();
    public string? ImdbId { get; set; }
}

public class UpdateMovieDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Poster { get; set; }
    public string? Backdrop { get; set; }
    public List<string>? StreamingProviders { get; set; }
}

public class MovieSearchResultDto
{
    public List<MovieDto> Movies { get; set; } = new List<MovieDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
