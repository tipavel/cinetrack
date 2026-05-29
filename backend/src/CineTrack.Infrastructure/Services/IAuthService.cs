using CineTrack.WebApi.DTOs;

namespace CineTrack.Infrastructure.Services;

public interface IAuthService
{
    Task<AuthTokenDto?> RegisterAsync(UserRegistrationDto dto);
    Task<AuthTokenDto?> LoginAsync(UserLoginDto dto);
    Task<AuthTokenDto?> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(int userId);
    string GenerateAccessToken(UserProfileDto user);
    string GenerateRefreshToken();
    int? GetUserIdFromToken(string token);
}
