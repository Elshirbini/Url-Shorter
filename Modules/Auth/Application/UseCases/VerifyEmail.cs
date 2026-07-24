using System.Text.Json;
using UrlShorter.Common.Emails;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Redis;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;
using UrlShorter.Modules.Users.Infrastructure.Enums;
using UrlShorter.Modules.Users.Infrastructure.Models;

namespace UrlShorter.Modules.Auth.Application.UseCases;

public class VerifyEmailUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRedisClient _redis;

    public VerifyEmailUseCase(IUserRepository userRepository, IRedisClient redis)
    {
        _userRepository = userRepository;
        _redis = redis;
    }

    public async Task<ApiResponse<object>> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default)
    {
        var data = await _redis.GetAsync<TempSignupData>($"otp:{dto.Otp}") ?? throw new BadRequestException("OTP expired or invalid");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = data.Email,
            UserName = data.UserName,
            Password = hashedPassword,
            Role = data.Role,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.SaveUserAsync(user, cancellationToken);

        await _redis.DeleteAsync($"otp:{dto.Otp}");


        return new ApiResponse<object>
        {
            Success = true,
            Message = "Email verified and user created"
        };
    }

    public class TempSignupData
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public string Otp { get; set; } = string.Empty;
    }
}