using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Auth.Presentation.DTOs;

public class VerifyCodeDto
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;
}