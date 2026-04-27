using Microsoft.AspNetCore.Mvc;
using UrlShorter.Modules.Auth.DTOs;

namespace UrlShorter.Modules.Auth;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    // 🔐 LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(HttpContext, dto);
        return Ok(result);
    }

    // 📝 SIGNUP
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupDto dto)
    {
        var result = await _authService.SignupAsync(dto);
        return Ok(result);
    }

    // 📩 VERIFY EMAIL (OTP)
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        var result = await _authService.VerifyEmailAsync(dto);
        return Ok(result);
    }

    // 🔄 FORGET PASSWORD
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
    {
        var result = await _authService.ForgetPasswordAsync(dto);
        return Ok(result);
    }

    // 🔢 VERIFY CODE
    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto dto)
    {
        var result = await _authService.VerifyCodeAsync(dto);
        return Ok(result);
    }

    // 🔑 NEW PASSWORD
    [HttpPatch("new-password")]
    public async Task<IActionResult> NewPassword([FromBody] NewPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto);
        return Ok(result);
    }

    // 🔄 REFRESH TOKEN
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var result = await _authService.RefreshTokenAsync(HttpContext);
        return Ok(result);
    }

    // 🚪 LOGOUT
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync(HttpContext);
        return Ok(result);
    }
}