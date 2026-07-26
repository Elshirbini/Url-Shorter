using Microsoft.AspNetCore.Http;

namespace UrlShorter.Modules.Users.Presentation.DTOs;

public class UpdateUserProfileDto
{
    public string UserName { get; set; } = string.Empty;

    public IFormFile? ProfileImage { get; set; }
}