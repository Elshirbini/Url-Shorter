using UrlShorter.Common.Emails;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Security;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases;


public class ForgetPasswordUseCase
{

    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ForgetPasswordUseCase(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<ApiResponse<object>> ForgetPasswordAsync(ForgetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetFirstOrDefaultUserAsync(u => u.Email == dto.Email, cancellationToken) ?? throw new NotFoundException("User not found");

        var code = CodeGenerator.Generate(6);

        var hashedCode = BCrypt.Net.BCrypt.HashPassword(code);

        user.CodeValidation = hashedCode;
        user.CodeValidationExpire = DateTime.UtcNow.AddMinutes(10);

        await _userRepository.SaveUserChangesAsync(cancellationToken);

        await _emailService.SendResetPasswordAsync(user.Email, code, cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Verification code sent to email"
        };
    }
}