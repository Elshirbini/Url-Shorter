using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Auth.Presentation.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Identifier is required")]
    public string Identifier { get; set; } = string.Empty; // email أو username

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}