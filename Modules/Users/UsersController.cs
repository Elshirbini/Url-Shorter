using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.Modules.Users.DTOs;

namespace UrlShorter.Modules.Users;

[ApiController]
[Route("api/v1/user")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;

        return Guid.Parse(userId!);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var result = await _userService.GetUserAsync(GetUserId());
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateUserName(UpdateUserNameDto dto)
    {
        var result = await _userService.UpdateUserNameAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await _userService.ResetPasswordAsync(GetUserId(), dto);
        return Ok(result);
    }
}