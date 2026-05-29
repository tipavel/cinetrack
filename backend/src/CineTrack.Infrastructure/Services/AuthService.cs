using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CineTrack.Domain.Entities;
using CineTrack.Infrastructure.Data;
using CineTrack.Infrastructure.Repositories;
using CineTrack.WebApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CineTrack.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly CineTrackDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(CineTrackDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthTokenDto?> RegisterAsync(UserRegistrationDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return null;

        if (dto.Password != dto.ConfirmPassword)
            return null;

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email || u.Username == dto.Username);

        if (existingUser != null)
            return null;

        // Create new user
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userProfile = MapToUserProfileDto(user);
        var accessToken = GenerateAccessToken(userProfile);
        var refreshToken = GenerateRefreshToken();

        return new AuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = userProfile
        };
    }

    public async Task<AuthTokenDto?> LoginAsync(UserLoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            return null;

        var userProfile = MapToUserProfileDto(user);
        var accessToken = GenerateAccessToken(userProfile);
        var refreshToken = GenerateRefreshToken();

        return new AuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = userProfile
        };
    }

    public async Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken)
    {
        // In a production app, you would validate the refresh token against a database
        // For now, this is a placeholder implementation
        await Task.CompletedTask;
        return null;
    }

    public async Task LogoutAsync(int userId)
    {
        // Implement logout logic (e.g., invalidate tokens)
        await Task.CompletedTask;
    }

    public string GenerateAccessToken(UserProfileDto user)
    {
        var jwtSecret = _configuration["Jwt:Secret"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var jwtExpirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret ?? "DefaultSecretKeyThatIsVeryLongAndSecure123!@#"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public int? GetUserIdFromToken(string token)
    {
        try
        {
            var jwtSecret = _configuration["Jwt:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret ?? "DefaultSecretKeyThatIsVeryLongAndSecure123!@#"));
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
        catch
        {
            return null;
        }
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = Convert.ToBase64String(
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        return hashOfInput == hash;
    }

    private UserProfileDto MapToUserProfileDto(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Bio = user.Bio,
            Location = user.Location,
            CreatedAt = user.CreatedAt,
            TotalReviews = user.TotalReviews,
            AverageRating = user.AverageRating,
            FollowersCount = user.Followers.Count,
            FollowingCount = user.Following.Count
        };
    }
}
