using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases;


public class NewPasswordUseCase
{
    private readonly IUserRepository _userRepository;

    public NewPasswordUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(NewPasswordDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new BadRequestException("Passwords do not match");

        var user = await _userRepository.GetFirstOrDefaultUserAsync(u => u.PasswordResetToken == dto.ResetToken) ?? throw new BadRequestException("Invalid reset token");

        if (user.PasswordResetTokenExpire < DateTime.UtcNow)
            throw new BadRequestException("Reset token expired");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        user.Password = hashedPassword;

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpire = null;

        await _userRepository.SaveUserChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Password reset successfully"
        };
    }

}