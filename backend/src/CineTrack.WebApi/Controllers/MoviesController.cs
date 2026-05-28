using CineTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CineTrack.Domain.Entities;

namespace CineTrack.WebApi.Controllers
{
    /// <summary>
    /// Movies endpoints - Search, filtering, and details
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly CineTrackDbContext _context;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(CineTrackDbContext context, ILogger<MoviesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Search movies by title
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { error = "Search query is required" });

                var movies = await _context.Movies
                    .Where(m => m.Title.Contains(query))
                    .OrderByDescending(m => m.Rating)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new
                    {
                        m.Id,
                        m.TmdbId,
                        m.Title,
                        m.PosterUrl,
                        m.Rating,
                        m.ReleaseDate,
                        m.Runtime,
                        m.Genres
                    })
                    .ToListAsync();

                _logger.LogInformation($"🔍 Search for: {query} - Found {movies.Count} movies");
                return Ok(new { data = movies, page, pageSize, total = await _context.Movies.CountAsync(m => m.Title.Contains(query)) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Search error: {ex.Message}\");\n                return StatusCode(500, new { error = \"An error occurred\" });\n            }\n        }\n\n        /// <summary>\n        /// Get movie details\n        /// </summary>\n        [HttpGet(\"{id}\")]\n        public async Task<IActionResult> GetMovie(int id)\n        {\n            try\n            {\n                var movie = await _context.Movies\n                    .Include(m => m.StreamingProviders)\n                    .FirstOrDefaultAsync(m => m.Id == id);\n\n                if (movie == null)\n                    return NotFound(new { error = \"Movie not found\" });\n\n                return Ok(new\n                {\n                    movie.Id,\n                    movie.TmdbId,\n                    movie.Title,\n                    movie.Description,\n                    movie.PosterUrl,\n                    movie.BackdropUrl,\n                    movie.Rating,\n                    movie.VoteCount,\n                    movie.ReleaseDate,\n                    movie.Runtime,\n                    movie.Genres,\n                    movie.Cast,\n                    movie.DirectorName,\n                    movie.Language,\n                    movie.AverageUserRating,\n                    movie.UserRatingCount,\n                    StreamingProviders = movie.StreamingProviders.Select(sp => new\n                    {\n                        sp.ProviderName,\n                        sp.StreamingUrl,\n                        sp.AvailableFrom,\n                        sp.AvailableUntil\n                    })\n                });\n            }\n            catch (Exception ex)\n            {\n                _logger.LogError($\"Get movie error: {ex.Message}\");\n                return StatusCode(500, new { error = \"An error occurred\" });\n            }\n        }\n\n        /// <summary>\n        /// Get trending movies\n        /// </summary>\n        [HttpGet(\"trending\")]\n        public async Task<IActionResult> GetTrendingMovies([FromQuery] string timeWindow = \"week\", [FromQuery] int limit = 20)\n        {\n            try\n            {\n                var movies = await _context.Movies\n                    .OrderByDescending(m => m.AverageUserRating)\n                    .ThenByDescending(m => m.UserRatingCount)\n                    .Take(limit)\n                    .Select(m => new\n                    {\n                        m.Id,\n                        m.TmdbId,\n                        m.Title,\n                        m.PosterUrl,\n                        m.Rating,\n                        m.ReleaseDate,\n                        m.Genres,\n                        m.AverageUserRating\n                    })\n                    .ToListAsync();\n\n                _logger.LogInformation($\"📈 Retrieved {movies.Count} trending movies\");\n                return Ok(movies);\n            }\n            catch (Exception ex)\n            {\n                _logger.LogError($\"Get trending error: {ex.Message}\");\n                return StatusCode(500, new { error = \"An error occurred\" });\n            }\n        }\n\n        /// <summary>\n        /// Get movies by genre\n        /// </summary>\n        [HttpGet(\"genre/{genre}\")]\n        public async Task<IActionResult> GetMoviesByGenre(string genre, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)\n        {\n            try\n            {\n                var movies = await _context.Movies\n                    .Where(m => m.Genres.Contains(genre))\n                    .OrderByDescending(m => m.Rating)\n                    .Skip((page - 1) * pageSize)\n                    .Take(pageSize)\n                    .Select(m => new\n                    {\n                        m.Id,\n                        m.TmdbId,\n                        m.Title,\n                        m.PosterUrl,\n                        m.Rating,\n                        m.ReleaseDate,\n                        m.Genres\n                    })\n                    .ToListAsync();\n\n                _logger.LogInformation($\"🎬 Retrieved {movies.Count} movies for genre: {genre}\");\n                return Ok(new { data = movies, page, pageSize });\n            }\n            catch (Exception ex)\n            {\n                _logger.LogError($\"Get movies by genre error: {ex.Message}\");\n                return StatusCode(500, new { error = \"An error occurred\" });\n            }\n        }\n\n        /// <summary>\n        /// Get streaming availability for a movie\n        /// </summary>\n        [HttpGet(\"{movieId}/streaming\")]\n        public async Task<IActionResult> GetStreamingAvailability(int movieId)\n        {\n            try\n            {\n                var providers = await _context.StreamingProviders\n                    .Where(sp => sp.MovieId == movieId)\n                    .Select(sp => new\n                    {\n                        sp.ProviderName,\n                        sp.StreamingUrl,\n                        sp.AvailableFrom,\n                        sp.AvailableUntil,\n                        sp.Regions\n                    })\n                    .ToListAsync();\n\n                _logger.LogInformation($\"📺 Found {providers.Count} streaming providers for movie {movieId}\");\n                return Ok(providers);\n            }\n            catch (Exception ex)\n            {\n                _logger.LogError($\"Get streaming error: {ex.Message}\");\n                return StatusCode(500, new { error = \"An error occurred\" });\n            }\n        }\n    }\n}\n"