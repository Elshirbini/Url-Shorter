using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Users.DTOs;

public class UpdateUserNameDto
{
    [Required(ErrorMessage = "UserName is required")]
    [MinLength(3, ErrorMessage = "UserName must be at least 3 characters long")]
    [MaxLength(50, ErrorMessage = "UserName must be at most 50 characters long")]
    public string UserName { get; set; } = string.Empty;
}