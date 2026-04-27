using UrlShorter.Modules.Auth.DTOs;
using UrlShorter.Common.Responses;

namespace UrlShorter.Modules.Auth;

public interface IAuthService
{
    Task<ApiResponse<object>> LoginAsync(HttpContext context, LoginDto dto);

    Task<ApiResponse<object>> SignupAsync(SignupDto dto);

    Task<ApiResponse<object>> VerifyEmailAsync(VerifyEmailDto dto);

    Task<ApiResponse<object>> ForgetPasswordAsync(ForgetPasswordDto dto);

    Task<ApiResponse<object>> VerifyCodeAsync(VerifyCodeDto dto);

    Task<ApiResponse<object>> ResetPasswordAsync(NewPasswordDto dto);

    Task<ApiResponse<object>> RefreshTokenAsync(HttpContext context);

    Task<ApiResponse<object>> LogoutAsync(HttpContext context);
}