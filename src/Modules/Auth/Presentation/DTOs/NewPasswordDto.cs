namespace UrlShorter.src.Modules.Auth.Presentation.DTOs;

public class NewPasswordDto
{
    public string ResetToken { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}