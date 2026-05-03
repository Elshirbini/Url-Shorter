using UrlShorter.Common.Responses;
using UrlShorter.Modules.Users.DTOs;

namespace UrlShorter.Modules.Users;

public interface IUserService
{
    Task<ApiResponse<object>> GetUserAsync(Guid userId);
    Task<ApiResponse<object>> UpdateUserNameAsync(Guid userId, UpdateUserNameDto dto);
    Task<ApiResponse<object>> ResetPasswordAsync(HttpContext context, Guid userId, ResetPasswordDto dto);
}