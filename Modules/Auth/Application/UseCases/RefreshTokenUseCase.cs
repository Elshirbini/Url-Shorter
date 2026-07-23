using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Security;
using UrlShorter.Modules.Auth.Application.Interfaces;
using UrlShorter.Modules.Auth.Infrastructure.Models;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases;


public class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenUseCase(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<object>> RefreshTokenAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var token = context.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedException("Missing refresh token");

        var principal = _tokenService.ValidateRefreshToken(token) ?? throw new UnauthorizedException("Invalid refresh token");

        var userId = Guid.Parse(
            principal.FindFirst("userId")!.Value
        );

        var jti = principal.FindFirst("jti")!.Value;

        var storedToken = await _refreshTokenRepository.GetFirstOrDefaultRefreshTokenAsync(x => x.Jti == jti, cancellationToken);

        if (storedToken == null || storedToken.RevokedAt != null || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token");

        storedToken.RevokedAt = DateTime.UtcNow;

        var user = await _userRepository.GetFirstOrDefaultUserAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new UnauthorizedException("User not found");

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var (newRefreshToken, newJti) = _tokenService.GenerateRefreshToken(user);

        await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = userId,
            Jti = newJti,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }, cancellationToken);

        await _userRepository.SaveUserChangesAsync(cancellationToken);

        context.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true
            // HttpOnly = true,
            // Secure = true,
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        context.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true
            // HttpOnly = true,
            // Secure = true, 
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Token refreshed"
        };
    }
}
