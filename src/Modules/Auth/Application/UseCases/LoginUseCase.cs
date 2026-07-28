using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Common.Security;
using UrlShorter.src.Modules.Auth.Application.Interfaces;
using UrlShorter.src.Modules.Auth.Infrastructure.Models;
using UrlShorter.src.Modules.Auth.Presentation.DTOs;
using UrlShorter.src.Modules.Users.Application.Interfaces;

namespace UrlShorter.src.Modules.Auth.Application.UseCases;


public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginUseCase(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<object>> LoginAsync(HttpContext context, LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetFirstOrDefaultUserAsync(u =>
                u.Email == dto.Identifier || u.UserName == dto.Identifier,
                cancellationToken
            ) ?? throw new UnauthorizedException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UnauthorizedException("Wrong password");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var (refreshToken, jti) = _tokenService.GenerateRefreshToken(user);

        await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = user.UserId,
            Jti = jti,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }, cancellationToken);


        context.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
            // HttpOnly = true,
            // Secure = true, 
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        context.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
            // HttpOnly = true,
            // Secure = true, 
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Login successful"
        };
    }
}