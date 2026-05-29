using CineTrack.Domain.Entities;
using CineTrack.WebApi.DTOs;

namespace CineTrack.Infrastructure.Services;

public interface IMovieService
{
    Task<MovieDto?> GetMovieByIdAsync(int id);
    Task<MovieSearchResultDto> SearchMoviesAsync(string query, int page = 1, int pageSize = 20);
    Task<MovieSearchResultDto> GetMoviesByGenreAsync(string genre, int page = 1, int pageSize = 20);
    Task<MovieSearchResultDto> GetMoviesByStreamingProviderAsync(string provider, int page = 1, int pageSize = 20);
    Task<MovieDto> CreateMovieAsync(CreateMovieDto dto);
    Task<MovieDto?> UpdateMovieAsync(int id, UpdateMovieDto dto);
    Task<bool> DeleteMovieAsync(int id);
    Task<List<MovieDto>> GetTrendingMoviesAsync(int count = 10);
    Task<List<MovieDto>> GetTopRatedMoviesAsync(int count = 10);
}
