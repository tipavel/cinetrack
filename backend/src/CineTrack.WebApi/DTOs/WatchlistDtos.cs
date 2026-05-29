namespace CineTrack.WebApi.DTOs;

public class WatchlistDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MovieCount { get; set; }
    public List<MovieDto>? Movies { get; set; }
}

public class CreateWatchlistDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
}

public class UpdateWatchlistDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
}

public class AddMovieToWatchlistDto
{
    public int MovieId { get; set; }
}
