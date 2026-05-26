using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CineTrack.Infrastructure.Services
{
    /// <summary>
    /// Service for integrating with The Movie Database (TMDb) API
    /// </summary>
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TmdbService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public TmdbService(HttpClient httpClient, IConfiguration configuration, ILogger<TmdbService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = configuration["TmdbApi:ApiKey"] ?? throw new InvalidOperationException("TMDb API key not configured");
            _baseUrl = configuration["TmdbApi:BaseUrl"] ?? "https://api.themoviedb.org/3";
        }

        public async Task<dynamic?> SearchMoviesAsync(string query, int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsAsync<dynamic>();
                _logger.LogInformation($"TMDb search for: {query}");
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError($"TMDb search error: {ex.Message}");
                throw;
            }
        }

        public async Task<dynamic?> GetMovieDetailsAsync(int tmdbId)
        {
            try
            {
                var url = $"{_baseUrl}/movie/{tmdbId}?api_key={_apiKey}&append_to_response=credits,videos,recommendations";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadAsAsync<dynamic>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"TMDb movie details error: {ex.Message}");
                throw;
            }
        }

        public async Task<dynamic?> GetTrendingMoviesAsync(string timeWindow = "week")
        {
            try
            {
                var url = $"{_baseUrl}/trending/movie/{timeWindow}?api_key={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadAsAsync<dynamic>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"TMDb trending movies error: {ex.Message}");
                throw;
            }
        }

        public async Task<dynamic?> GetMoviesByGenreAsync(int genreId, int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/discover/movie?api_key={_apiKey}&with_genres={genreId}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadAsAsync<dynamic>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"TMDb genre movies error: {ex.Message}");
                throw;
            }
        }
    }

    public interface ITmdbService
    {
        Task<dynamic?> SearchMoviesAsync(string query, int page = 1);
        Task<dynamic?> GetMovieDetailsAsync(int tmdbId);
        Task<dynamic?> GetTrendingMoviesAsync(string timeWindow = "week");
        Task<dynamic?> GetMoviesByGenreAsync(int genreId, int page = 1);
    }
}