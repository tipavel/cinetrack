using CineTrack.Domain.Entities;
using CineTrack.Infrastructure.Data;
using CineTrack.Infrastructure.Repositories;
using CineTrack.WebApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CineTrack.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly CineTrackDbContext _context;

    public MovieService(CineTrackDbContext context)
    {
        _context = context;
    }

    public async Task<MovieDto?> GetMovieByIdAsync(int id)
    {
        var movie = await _context.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
            return null;

        movie.ViewCount++;
        await _context.SaveChangesAsync();

        return MapToMovieDto(movie);
    }

    public async Task<MovieSearchResultDto> SearchMoviesAsync(string query, int page = 1, int pageSize = 20)
    {
        var lowerQuery = query.ToLower();
        var movies = await _context.Movies
            .Where(m => m.Title.ToLower().Contains(lowerQuery) || 
                       m.Description.ToLower().Contains(lowerQuery) ||
                       m.Director.ToLower().Contains(lowerQuery) ||
                       m.Cast.Any(c => c.ToLower().Contains(lowerQuery)))
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.ViewCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _context.Movies
            .CountAsync(m => m.Title.ToLower().Contains(lowerQuery) || 
                            m.Description.ToLower().Contains(lowerQuery) ||
                            m.Director.ToLower().Contains(lowerQuery) ||
                            m.Cast.Any(c => c.ToLower().Contains(lowerQuery)));

        return new MovieSearchResultDto
        {
            Movies = movies.Select(MapToMovieDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<MovieSearchResultDto> GetMoviesByGenreAsync(string genre, int page = 1, int pageSize = 20)
    {
        var movies = await _context.Movies
            .Where(m => m.Genres.Contains(genre))
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.AverageUserRating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _context.Movies
            .CountAsync(m => m.Genres.Contains(genre));

        return new MovieSearchResultDto
        {
            Movies = movies.Select(MapToMovieDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<MovieSearchResultDto> GetMoviesByStreamingProviderAsync(string provider, int page = 1, int pageSize = 20)
    {
        var movies = await _context.Movies
            .Where(m => m.StreamingProviders.Contains(provider))
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.AverageUserRating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _context.Movies
            .CountAsync(m => m.StreamingProviders.Contains(provider));

        return new MovieSearchResultDto
        {
            Movies = movies.Select(MapToMovieDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto dto)
    {
        var movie = new Movie
        {
            Title = dto.Title,
            Description = dto.Description,
            Poster = dto.Poster,
            Backdrop = dto.Backdrop,
            Rating = dto.Rating,
            ReleaseYear = dto.ReleaseYear,
            Runtime = dto.Runtime,
            Director = dto.Director,
            Genres = dto.Genres ?? new List<string>(),
            Cast = dto.Cast ?? new List<string>(),
            StreamingProviders = dto.StreamingProviders ?? new List<string>(),
            ImdbId = dto.ImdbId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ViewCount = 0
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return MapToMovieDto(movie);
    }

    public async Task<MovieDto?> UpdateMovieAsync(int id, UpdateMovieDto dto)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Title))
            movie.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description))
            movie.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.Poster))
            movie.Poster = dto.Poster;
        if (!string.IsNullOrEmpty(dto.Backdrop))
            movie.Backdrop = dto.Backdrop;
        if (dto.StreamingProviders != null)
            movie.StreamingProviders = dto.StreamingProviders;

        movie.UpdatedAt = DateTime.UtcNow;

        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();

        return MapToMovieDto(movie);
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
            return false;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MovieDto>> GetTrendingMoviesAsync(int count = 10)
    {
        var movies = await _context.Movies
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.ViewCount)
            .Take(count)
            .ToListAsync();

        return movies.Select(MapToMovieDto).ToList();
    }

    public async Task<List<MovieDto>> GetTopRatedMoviesAsync(int count = 10)
    {
        var movies = await _context.Movies
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.AverageUserRating)
            .Take(count)
            .ToListAsync();

        return movies.Select(MapToMovieDto).ToList();
    }

    private MovieDto MapToMovieDto(Movie movie)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Poster = movie.Poster,
            Backdrop = movie.Backdrop,
            Rating = movie.Rating,
            ReleaseYear = movie.ReleaseYear,
            Runtime = movie.Runtime,
            Director = movie.Director,
            Genres = movie.Genres,
            Cast = movie.Cast,
            StreamingProviders = movie.StreamingProviders,
            ImdbRating = movie.ImdbRating,
            AverageUserRating = movie.AverageUserRating,
            TotalReviews = movie.TotalReviews,
            ViewCount = movie.ViewCount
        };
    }
}
