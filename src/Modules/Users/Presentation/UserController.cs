using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.src.Modules.Users.Application.UseCases;
using UrlShorter.src.Modules.Users.Presentation.DTOs;

namespace UrlShorter.src.Modules.Users.Presentation;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly GetUserUseCase _getUserUseCase;
    private readonly UpdateUserProfileUseCase _updateUserProfileUseCase;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;

    public UserController(GetUserUseCase getUserUseCase, UpdateUserProfileUseCase updateUserProfileUseCase, ResetPasswordUseCase resetPasswordUseCase)
    {
        _getUserUseCase = getUserUseCase;
        _updateUserProfileUseCase = updateUserProfileUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;

        return Guid.Parse(userId!);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(CancellationToken cancellationToken)
    {
        var result = await _getUserUseCase.GetUserAsync(GetUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpPatch]
    [RequestFormLimits(MultipartBodyLengthLimit = 5 * 1024 * 1024)]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserProfileDto dto, CancellationToken cancellationToken)
    {
        var result = await _updateUserProfileUseCase.UpdateProfileAsync(GetUserId(), dto, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, CancellationToken cancellationToken)
    {
        var result = await _resetPasswordUseCase.ResetPasswordAsync(HttpContext, GetUserId(), dto, cancellationToken);
        return Ok(result);
    }
}