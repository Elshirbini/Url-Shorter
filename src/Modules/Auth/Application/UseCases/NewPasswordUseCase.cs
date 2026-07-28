using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Auth.Presentation.DTOs;
using UrlShorter.src.Modules.Users.Application.Interfaces;

namespace UrlShorter.src.Modules.Auth.Application.UseCases;


public class NewPasswordUseCase
{
    private readonly IUserRepository _userRepository;

    public NewPasswordUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(NewPasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new BadRequestException("Passwords do not match");

        var user = await _userRepository.GetFirstOrDefaultUserAsync(u => u.PasswordResetToken == dto.ResetToken, cancellationToken) ?? throw new BadRequestException("Invalid reset token");

        if (user.PasswordResetTokenExpire < DateTime.UtcNow)
            throw new BadRequestException("Reset token expired");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        user.Password = hashedPassword;

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpire = null;

        await _userRepository.SaveUserChangesAsync(cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Password reset successfully"
        };
    }

}