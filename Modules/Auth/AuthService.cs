using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Services;
using UrlShorter.Data;
using UrlShorter.Modules.Auth.DTOs;
using UrlShorter.Modules.Users.Models;
using UrlShorter.Modules.Auth.Models;
using UrlShorter.Common.Emails.Interfaces;
using UrlShorter.Common.Emails.Templates;
using UrlShorter.Common.Emails;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.Common.Security;

namespace UrlShorter.Modules.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AuthService> _logger;
    private readonly RedisService _redis;
    private readonly IEmailService _emailService;

    private readonly ITokenService _tokenService;



    public AuthService(AppDbContext db, ILogger<AuthService> logger, RedisService redis, IEmailService emailService, ITokenService tokenService)
    {
        _db = db;
        _logger = logger;
        _redis = redis;
        _emailService = emailService;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<object>> SignupAsync(SignupDto dto)
    {
        var exists = await _db.Users
            .AnyAsync(u => u.Email == dto.Email || u.UserName == dto.UserName);

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

    public async Task<ApiResponse<object>> LoginAsync(HttpContext context, LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u =>
                u.Email == dto.Identifier || u.UserName == dto.Identifier
            ) ?? throw new UnauthorizedException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UnauthorizedException("Invalid credentials");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var (refreshToken, jti) = _tokenService.GenerateRefreshToken(user);

        await _db.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            Jti = jti,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _db.SaveChangesAsync();

        context.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
            // HttpOnly = true,
            // Secure = true, 
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        context.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
            // HttpOnly = true,
            // Secure = true, 
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Login successful"
        };
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

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        await _redis.DeleteAsync($"otp:{dto.Otp}");


        return new ApiResponse<object>
        {
            Success = true,
            Message = "Email verified and user created"
        };
    }

    public async Task<ApiResponse<object>> ForgetPasswordAsync(ForgetPasswordDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email) ?? throw new NotFoundException("User not found");

        var code = CodeGenerator.Generate(6);

        var hashedCode = BCrypt.Net.BCrypt.HashPassword(code);

        user.CodeValidation = hashedCode;
        user.CodeValidationExpire = DateTime.UtcNow.AddMinutes(10);

        await _db.SaveChangesAsync();

        await _emailService.SendResetPasswordAsync(user.Email, code);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Verification code sent to email"
        };
    }

    public async Task<ApiResponse<object>> VerifyCodeAsync(VerifyCodeDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || user.CodeValidation == null)
            throw new BadRequestException("Invalid code");

        if (user.CodeValidationExpire < DateTime.UtcNow)
            throw new BadRequestException("Code expired");

        var isValid = BCrypt.Net.BCrypt.Verify(dto.Code, user.CodeValidation);

        if (!isValid)
            throw new BadRequestException("Invalid code");

        var resetToken = Guid.NewGuid().ToString();

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpire = DateTime.UtcNow.AddMinutes(15);

        user.CodeValidation = null;
        user.CodeValidationExpire = null;

        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Code verified",
            Data = new { resetToken }
        };
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(NewPasswordDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new BadRequestException("Passwords do not match");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == dto.ResetToken) ?? throw new BadRequestException("Invalid reset token");

        if (user.PasswordResetTokenExpire < DateTime.UtcNow)
            throw new BadRequestException("Reset token expired");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        user.Password = hashedPassword;

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpire = null;

        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Password reset successfully"
        };
    }

    public async Task<ApiResponse<object>> RefreshTokenAsync(HttpContext context)
    {
        var token = context.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedException("Missing refresh token");

        var principal = _tokenService.ValidateRefreshToken(token) ?? throw new UnauthorizedException("Invalid refresh token");

        var userId = Guid.Parse(
            principal.FindFirst("userId")!.Value
        );

        var jti = principal.FindFirst("jti")!.Value;

        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.Jti == jti);

        if (storedToken == null || storedToken.RevokedAt != null || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token");

        storedToken.RevokedAt = DateTime.UtcNow;

        var user = await _db.Users.FindAsync(userId)
            ?? throw new UnauthorizedException("User not found");

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var (newRefreshToken, newJti) = _tokenService.GenerateRefreshToken(user);

        await _db.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            Jti = newJti,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _db.SaveChangesAsync();

        context.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true
            // HttpOnly = true,
            // Secure = true,
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        context.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true
            // HttpOnly = true,
            // Secure = true, 
            // SameSite = SameSiteMode.None, 
            // Expires = DateTime.UtcNow.AddDays(7)
        });

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Token refreshed"
        };
    }

    public async Task<ApiResponse<object>> LogoutAsync(HttpContext context)
    {
        var token = context.Request.Cookies["refreshToken"];

        if (token != null)
        {
            var principal = _tokenService.ValidateRefreshToken(token) ?? throw new UnauthorizedException("Invalid refresh token");

            if (principal != null)
            {
                var jti = principal.FindFirst("jti")?.Value;

                var stored = await _db.RefreshTokens
                    .FirstOrDefaultAsync(x => x.Jti == jti);

                if (stored != null)
                {
                    stored.RevokedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }
        }

        context.Response.Cookies.Delete("accessToken");
        context.Response.Cookies.Delete("refreshToken");

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Logged out"
        };
    }
}

public class TempSignupData
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}