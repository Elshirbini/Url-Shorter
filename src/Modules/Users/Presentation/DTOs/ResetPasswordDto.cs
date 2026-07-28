namespace UrlShorter.src.Modules.Users.Presentation.DTOs;

public class ResetPasswordDto
{
    public string OldPassword { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}