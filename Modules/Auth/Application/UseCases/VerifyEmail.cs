using System.Text.Json;
using UrlShorter.Common.Emails;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Services;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;
using UrlShorter.Modules.Users.Infrastructure.Models;

namespace UrlShorter.Modules.Auth.Application.UseCases;

public class VerifyEmailUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly RedisService _redis;

    public VerifyEmailUseCase(IUserRepository userRepository, RedisService redis)
    {
        _userRepository = userRepository;
        _redis = redis;
    }

    public async Task<ApiResponse<object>> VerifyEmailAsync(VerifyEmailDto dto)
    {
        var data = await _redis.GetAsync($"otp:{dto.Otp}") ?? throw new BadRequestException("OTP expired or invalid");
        var parsed = JsonSerializer.Deserialize<TempSignupData>(data) ?? throw new Exception("Invalid stored data");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(parsed.Password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = parsed.Email,
            UserName = parsed.UserName,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.SaveUserAsync(user);

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
        public string Otp { get; set; } = string.Empty;
    }
}