namespace UrlShorter.Modules.Auth.Presentation.DTOs;

public class LoginDto
{
    public string Identifier { get; set; } = string.Empty; // email أو username

    public string Password { get; set; } = string.Empty;
}