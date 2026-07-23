using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.Modules.Users.Application.UseCases;
using UrlShorter.Modules.Users.Presentation.DTOs;

namespace UrlShorter.Modules.Users.Presentation;

[ApiController]
[Route("api/v1/user")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly GetUserUseCase _getUserUseCase;
    private readonly UpdateUserProfileUseCase _updateUserProfileUseCase;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;

    public UsersController(GetUserUseCase getUserUseCase, UpdateUserProfileUseCase updateUserProfileUseCase, ResetPasswordUseCase resetPasswordUseCase)
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