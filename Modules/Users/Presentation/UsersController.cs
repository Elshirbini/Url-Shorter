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
    private readonly UpdateUserNameUseCase _updateUserNameUseCase;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;

    public UsersController(GetUserUseCase getUserUseCase, UpdateUserNameUseCase updateUserNameUseCase, ResetPasswordUseCase resetPasswordUseCase)
    {
        _getUserUseCase = getUserUseCase;
        _updateUserNameUseCase = updateUserNameUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;

        return Guid.Parse(userId!);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var result = await _getUserUseCase.GetUserAsync(GetUserId());
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateUserName(UpdateUserNameDto dto)
    {
        var result = await _updateUserNameUseCase.UpdateUserNameAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await _resetPasswordUseCase.ResetPasswordAsync(HttpContext, GetUserId(), dto);
        return Ok(result);
    }
}