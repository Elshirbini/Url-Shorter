using System.Text.Json;
using MassTransit;
using UrlShorter.src.Common.Messaging.Contracts;
using UrlShorter.src.Modules.Emails.Enums;
using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Common.Redis;
using UrlShorter.src.Modules.Auth.Presentation.DTOs;
using UrlShorter.src.Modules.Users.Application.Interfaces;
using UrlShorter.src.Modules.Users.Infrastructure.Enums;

namespace UrlShorter.src.Modules.Auth.Application.UseCases;

public class SignupUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRedisClient _redis;

    public SignupUseCase(IUserRepository userRepository, IPublishEndpoint publishEndpoint, IRedisClient redis)
    {
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
        _redis = redis;
    }

    public async Task<ApiResponse<object>> SignupAsync(SignupDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _userRepository.UserExistsAsync(u => u.Email == dto.Email || u.UserName == dto.UserName, cancellationToken);

        if (exists)
        {
            throw new ConflictException("Email or username already exists");
        }

        var otp = new Random().Next(100000, 999999).ToString();
        var userData = new
        {
            Username = dto.UserName,
            Email = dto.Email,
            Password = dto.Password,
            Role = UserRole.User
        };

        await _redis.SetAsync(
            $"otp:{otp}",
            JsonSerializer.Serialize(userData),
            TimeSpan.FromMinutes(10));

        await _publishEndpoint.Publish(new SendEmailMessage(
            dto.Email,
            EmailTemplate.SendOtpConfirmation,
            new Dictionary<string, object?> { { "Otp", otp } }
        ), cancellationToken);


        return new ApiResponse<object>
        {
            Success = true,
            Message = "OTP sent to email"
        };
    }
}