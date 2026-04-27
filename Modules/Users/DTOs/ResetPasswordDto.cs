using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Users.DTOs;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Old Password is required")]
    [MinLength(6, ErrorMessage = "Old Password must be at least 6 characters")]
    [MaxLength(16, ErrorMessage = "Old Password must not exceed 16 characters")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New Password is required")]
    [MinLength(6, ErrorMessage = "New Password must be at least 6 characters")]
    [MaxLength(16, ErrorMessage = "New Password must not exceed 16 characters")]
    public string NewPassword { get; set; } = string.Empty;
}