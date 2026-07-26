namespace UrlShorter.Modules.Auth.Presentation.DTOs;

public class SignupDto
{
    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}