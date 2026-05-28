using CineTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CineTrack.Domain.Entities;

namespace CineTrack.WebApi.Controllers
{
    /// <summary>
    /// Watchlist management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WatchlistsController : ControllerBase
    {
        private readonly CineTrackDbContext _context;
        private readonly ILogger<WatchlistsController> _logger;

        public WatchlistsController(CineTrackDbContext context, ILogger<WatchlistsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Get all watchlists for current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWatchlists()
        {
            try
            {
                var userId = GetUserId();
                var watchlists = await _context.Watchlists
                    .Where(w => w.UserId == userId)
                    .Select(w => new
                    {
                        w.Id,
                        w.Name,
                        w.Description,
                        w.Type,
                        w.IsPublic,
                        ItemCount = w.Items.Count,
                        w.CreatedAt,
                        w.UpdatedAt
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Retrieved {watchlists.Count} watchlists for user {userId}");
                return Ok(watchlists);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get watchlists error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Get single watchlist with items
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWatchlist(int id)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _context.Watchlists
                    .Where(w => w.Id == id && w.UserId == userId)
                    .Include(w => w.Items)
                    .ThenInclude(i => i.Movie)
                    .FirstOrDefaultAsync();

                if (watchlist == null)
                    return NotFound(new { error = "Watchlist not found" });

                return Ok(new
                {
                    watchlist.Id,
                    watchlist.Name,
                    watchlist.Description,
                    watchlist.Type,
                    watchlist.IsPublic,
                    watchlist.CreatedAt,
                    Items = watchlist.Items.OrderBy(i => i.Position).Select(i => new
                    {
                        i.Id,
                        i.MovieId,
                        Movie = new
                        {
                            i.Movie!.Id,
                            i.Movie.TmdbId,
                            i.Movie.Title,
                            i.Movie.PosterUrl,
                            i.Movie.Rating,
                            i.Movie.ReleaseDate
                        },
                        i.AddedAt
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get watchlist error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Create a new custom watchlist
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateWatchlist([FromBody] CreateWatchlistRequest request)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = new Watchlist
                {
                    UserId = userId,
                    Name = request.Name,
                    Description = request.Description,
                    Type = WatchlistType.Custom,
                    IsPublic = request.IsPublic,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Watchlists.Add(watchlist);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Watchlist created: {watchlist.Name} (ID: {watchlist.Id})");
                return CreatedAtAction(nameof(GetWatchlist), new { id = watchlist.Id }, watchlist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create watchlist error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Add movie to watchlist
        /// </summary>
        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddMovieToWatchlist(int id, [FromBody] AddMovieRequest request)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _context.Watchlists
                    .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

                if (watchlist == null)
                    return NotFound(new { error = "Watchlist not found" });

                // Check if movie already in watchlist
                if (await _context.WatchlistItems.AnyAsync(i => i.WatchlistId == id && i.MovieId == request.MovieId))
                    return BadRequest(new { error = "Movie already in watchlist" });

                var maxPosition = await _context.WatchlistItems
                    .Where(i => i.WatchlistId == id)
                    .MaxAsync(i => (int?)i.Position) ?? 0;

                var item = new WatchlistItem
                {
                    WatchlistId = id,
                    MovieId = request.MovieId,
                    Position = maxPosition + 1,
                    AddedAt = DateTime.UtcNow
                };

                _context.WatchlistItems.Add(item);
                watchlist.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Movie {request.MovieId} added to watchlist {id}");
                return CreatedAtAction(nameof(GetWatchlist), new { id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Add movie to watchlist error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Remove movie from watchlist
        /// </summary>
        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> RemoveMovieFromWatchlist(int id, int itemId)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _context.Watchlists
                    .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

                if (watchlist == null)
                    return NotFound(new { error = "Watchlist not found" });

                var item = await _context.WatchlistItems.FindAsync(itemId);
                if (item == null || item.WatchlistId != id)
                    return NotFound(new { error = "Item not found" });

                _context.WatchlistItems.Remove(item);
                watchlist.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Item {itemId} removed from watchlist {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Remove movie from watchlist error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Reorder movies in watchlist (drag-drop support)
        /// </summary>
        [HttpPut("{id}/reorder")]
        public async Task<IActionResult> ReorderWatchlist(int id, [FromBody] ReorderRequest request)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _context.Watchlists
                    .Where(w => w.Id == id && w.UserId == userId)
                    .Include(w => w.Items)
                    .FirstOrDefaultAsync();

                if (watchlist == null)
                    return NotFound(new { error = "Watchlist not found" });

                // Update positions
                for (int i = 0; i < request.ItemIds.Count; i++)
                {
                    var item = watchlist.Items.FirstOrDefault(it => it.Id == request.ItemIds[i]);
                    if (item != null)
                        item.Position = i;
                }

                watchlist.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Watchlist {id} reordered with {request.ItemIds.Count} items");
                return Ok(new { message = "Watchlist reordered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reorder watchlist error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Delete a watchlist
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWatchlist(int id)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _context.Watchlists
                    .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

                if (watchlist == null)
                    return NotFound(new { error = "Watchlist not found" });

                // Cannot delete system watchlists
                if (watchlist.Type != WatchlistType.Custom)
                    return BadRequest(new { error = "Cannot delete system watchlists" });

                _context.Watchlists.Remove(watchlist);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Watchlist {id} deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete watchlist error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }

    public record CreateWatchlistRequest(string Name, string? Description, bool IsPublic);
    public record AddMovieRequest(int MovieId);
    public record ReorderRequest(List<int> ItemIds);
}
