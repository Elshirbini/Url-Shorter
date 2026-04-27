using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Auth.DTOs;

public class VerifyEmailDto
{
    [Required(ErrorMessage = "OTP is required")]
    [MaxLength(6, ErrorMessage = "OTP must be at most 6 characters")]
    public string Otp { get; set; } = string.Empty;
}