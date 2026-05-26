using CineTrack.Infrastructure.Data;
using CineTrack.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CineTrack.WebApi.Controllers
{
    /// <summary>
    /// Authentication and authorization endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly CineTrackDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ITokenService tokenService, 
            CineTrackDbContext context, ILogger<AuthController> logger)
        {
            _authService = authService;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _authService.RegisterAsync(request.Email, request.Username, request.Password);
                
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accessToken,
                    refreshToken,
                    user = new { user.Id, user.Email, user.Username }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.AuthenticateAsync(request.Email, request.Password);
                if (user == null)
                    return Unauthorized(new { error = "Invalid credentials" });

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accessToken,
                    refreshToken,
                    user = new { user.Id, user.Email, user.Username }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
                if (principal == null)
                    return Unauthorized(new { error = "Invalid token" });

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                    return Unauthorized(new { error = "Invalid token claims" });

                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.RefreshToken != request.RefreshToken || 
                    user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    return Unauthorized(new { error = "Invalid refresh token" });
                }

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token refresh error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while refreshing token" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound();

                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.Username,
                    user.AvatarUrl,
                    user.Bio,
                    user.FavoriteGenres,
                    user.IsPublic,
                    user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get current user error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(request.Bio))
                    user.Bio = request.Bio;
                if (!string.IsNullOrEmpty(request.AvatarUrl))
                    user.AvatarUrl = request.AvatarUrl;
                if (request.FavoriteGenres != null)
                    user.FavoriteGenres = request.FavoriteGenres;
                
                user.IsPublic = request.IsPublic;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update profile error: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }

    public record RegisterRequest(string Email, string Username, string Password);
    public record LoginRequest(string Email, string Password);
    public record RefreshTokenRequest(string AccessToken, string RefreshToken);
    public record UpdateProfileRequest(string? Bio, string? AvatarUrl, List<string>? FavoriteGenres, bool IsPublic);
}