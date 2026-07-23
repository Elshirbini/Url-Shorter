using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Security;
using UrlShorter.Modules.Auth.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases;

public class LogoutUseCase
{

    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutUseCase(ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<object>> LogoutAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var token = context.Request.Cookies["refreshToken"];

        if (token != null)
        {
            var principal = _tokenService.ValidateRefreshToken(token) ?? throw new UnauthorizedException("Invalid refresh token");

            if (principal != null)
            {
                var jti = principal.FindFirst("jti")?.Value;

                var stored = await _refreshTokenRepository.GetFirstOrDefaultRefreshTokenAsync(x => x.Jti == jti, cancellationToken);

                if (stored != null)
                {
                    stored.RevokedAt = DateTime.UtcNow;
                    await _refreshTokenRepository.SaveRefreshTokenAsync(stored, cancellationToken);
                }
            }
        }

        context.Response.Cookies.Delete("accessToken");
        context.Response.Cookies.Delete("refreshToken");

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Logged out"
        };
    }
}