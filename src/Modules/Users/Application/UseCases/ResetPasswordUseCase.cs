using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Users.Application.Interfaces;
using UrlShorter.src.Modules.Users.Presentation.DTOs;

namespace UrlShorter.src.Modules.Users.Application.UseCases;

public class ResetPasswordUseCase
{
    private readonly IUserRepository _userRepository;

    public ResetPasswordUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(HttpContext context, Guid userId, ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserDocByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
            throw new UnauthorizedException("Old password is incorrect");

        var hashed = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        user.Password = hashed;

        await _userRepository.SaveUserChangesAsync(cancellationToken);

        context.Response.Cookies.Delete("accessToken");
        context.Response.Cookies.Delete("refreshToken");

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Password updated successfully"
        };
    }
}