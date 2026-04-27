using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Auth.DTOs;

public class ForgetPasswordDto
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;
}