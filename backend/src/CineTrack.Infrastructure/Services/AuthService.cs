using CineTrack.Domain.Entities;
using CineTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CineTrack.Infrastructure.Services
{
    /// <summary>
    /// Service for user authentication and password management
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly CineTrackDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(CineTrackDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
                {
                    _logger.LogWarning($"Failed login attempt for email: {email}");
                    return null;
                }

                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Authentication error: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> RegisterAsync(string email, string username, string password)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == email))
                    throw new InvalidOperationException("Email already registered");

                if (await _context.Users.AnyAsync(u => u.Username == username))
                    throw new InvalidOperationException("Username already taken");

                var user = new User
                {
                    Email = email,
                    Username = username,
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.UtcNow,
                    IsEmailVerified = false // In production, send verification email
                };

                _context.Users.Add(user);
                
                // Create default watchlists
                var toWatchList = new Watchlist
                {
                    UserId = user.Id,
                    Name = "To Watch",
                    Type = WatchlistType.ToWatch,
                    IsPublic = false
                };

                var watchedList = new Watchlist
                {
                    UserId = user.Id,
                    Name = "Watched",
                    Type = WatchlistType.Watched,
                    IsPublic = false
                };

                var favoritesList = new Watchlist
                {
                    UserId = user.Id,
                    Name = "Favorites",
                    Type = WatchlistType.Favorites,
                    IsPublic = true
                };

                _context.Watchlists.AddRange(toWatchList, watchedList, favoritesList);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User registered: {email}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration error: {ex.Message}");
                throw;
            }
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPasswordHash(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            if (!VerifyPasswordHash(oldPassword, user.PasswordHash))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> RegisterAsync(string email, string username, string password);
        string HashPassword(string password);
        bool VerifyPasswordHash(string password, string hash);
        Task<bool> UpdatePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}