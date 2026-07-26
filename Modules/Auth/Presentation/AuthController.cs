using Microsoft.AspNetCore.Mvc;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using Microsoft.AspNetCore.RateLimiting;
using UrlShorter.Modules.Auth.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace UrlShorter.Modules.Auth.Presentation;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly SignupUseCase _signupUseCase;
    private readonly LoginUseCase _loginUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly VerifyEmailUseCase _verifyEmailUseCase;
    private readonly ForgetPasswordUseCase _forgetPasswordUseCase;
    private readonly VerifyCodeUseCase _verifyCodeUseCase;
    private readonly NewPasswordUseCase _NewPasswordUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;


    public AuthController(SignupUseCase signupUseCase, LoginUseCase loginUseCase, LogoutUseCase logoutUseCase, VerifyEmailUseCase verifyEmailUseCase, ForgetPasswordUseCase forgetPasswordUseCase, VerifyCodeUseCase verifyCodeUseCase, NewPasswordUseCase newPasswordUseCase, RefreshTokenUseCase refreshTokenUseCase)
    {
        _signupUseCase = signupUseCase;
        _loginUseCase = loginUseCase;
        _logoutUseCase = logoutUseCase;
        _verifyEmailUseCase = verifyEmailUseCase;
        _forgetPasswordUseCase = forgetPasswordUseCase;
        _verifyCodeUseCase = verifyCodeUseCase;
        _NewPasswordUseCase = newPasswordUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
    }


    // 🔐 LOGIN
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _loginUseCase.LoginAsync(HttpContext, dto, cancellationToken);
        return Ok(result);
    }

    // 📝 SIGNUP
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupDto dto, CancellationToken cancellationToken)
    {
        var result = await _signupUseCase.SignupAsync(dto, cancellationToken);
        return Ok(result);
    }

    // 📩 VERIFY EMAIL (OTP)
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto, CancellationToken cancellationToken)
    {
        var result = await _verifyEmailUseCase.VerifyEmailAsync(dto, cancellationToken);
        return Ok(result);
    }

    // 🔄 FORGET PASSWORD
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto, CancellationToken cancellationToken)
    {
        var result = await _forgetPasswordUseCase.ForgetPasswordAsync(dto, cancellationToken);
        return Ok(result);
    }

    // 🔢 VERIFY CODE
    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto, CancellationToken cancellationToken)
    {
        var result = await _verifyCodeUseCase.VerifyCodeAsync(dto, cancellationToken);
        return Ok(result);
    }

    // 🔑 NEW PASSWORD
    [HttpPatch("new-password")]
    public async Task<IActionResult> NewPassword([FromBody] NewPasswordDto dto, CancellationToken cancellationToken)
    {
        var result = await _NewPasswordUseCase.ResetPasswordAsync(dto, cancellationToken);
        return Ok(result);
    }

    // 🔄 REFRESH TOKEN
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var result = await _refreshTokenUseCase.RefreshTokenAsync(HttpContext, cancellationToken);
        return Ok(result);
    }

    // 🚪 LOGOUT
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await _logoutUseCase.LogoutAsync(HttpContext, cancellationToken);
        return Ok(result);
    }
}