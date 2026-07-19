using System.Text.Json;
using UrlShorter.Common.Emails;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Services;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases;

public class SignupUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly RedisService _redis;

    public SignupUseCase(IUserRepository userRepository, IEmailService emailService, RedisService redis)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _redis = redis;
    }

    public async Task<ApiResponse<object>> SignupAsync(SignupDto dto)
    {
        var exists = await _userRepository.UserExistsAsync(u => u.Email == dto.Email || u.UserName == dto.UserName);

        if (exists)
        {
            throw new ConflictException("Email or username already exists");
        }

        var otp = new Random().Next(100000, 999999).ToString();

        await _redis.SetAsync(
            $"otp:{otp}",
            JsonSerializer.Serialize(dto),
            TimeSpan.FromMinutes(10)
        );

        await _emailService.SendOtpAsync(dto.Email, otp);


        return new ApiResponse<object>
        {
            Success = true,
            Message = "OTP sent to email"
        };
    }
}