using MassTransit;
using UrlShorter.Common.Messaging.Contracts;
using UrlShorter.Modules.Emails.Enums;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Security;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases;


public class ForgetPasswordUseCase
{

    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ForgetPasswordUseCase(IUserRepository userRepository, IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ApiResponse<object>> ForgetPasswordAsync(ForgetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetFirstOrDefaultUserAsync(u => u.Email == dto.Email, cancellationToken) ?? throw new NotFoundException("User not found");

        var code = CodeGenerator.Generate(6);

        var hashedCode = BCrypt.Net.BCrypt.HashPassword(code);

        user.CodeValidation = hashedCode;
        user.CodeValidationExpire = DateTime.UtcNow.AddMinutes(10);

        await _userRepository.SaveUserChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new SendEmailMessage(
            user.Email,
            EmailTemplate.SendResetPassword,
            new Dictionary<string, object?> { { "Code", code } }
        ), cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Verification code sent to email"
        };
    }
}