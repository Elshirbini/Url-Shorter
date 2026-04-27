using System.ComponentModel.DataAnnotations;
namespace UrlShorter.Modules.Auth.DTOs;

public class NewPasswordDto
{
    [Required(ErrorMessage = "Reset token is required")]
    public string ResetToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(16, ErrorMessage = "Password must not exceed 16 characters")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "Confirm Password is required")]
    [MinLength(6, ErrorMessage = "Confirm Password must be at least 6 characters")]
    [MaxLength(16, ErrorMessage = "Confirm Password must not exceed 16 characters")]
    public string ConfirmPassword { get; set; } = string.Empty;
}