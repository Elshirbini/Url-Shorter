using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Links.DTOs;

public class CreateLinkDto
{
    [StringLength(6, ErrorMessage = "Code must be 6 characters")]
    public string? Code { get; set; } // optional (custom)

    [Required]
    [Url(ErrorMessage = "Invalid URL")]
    public string RedirectUrl { get; set; } = string.Empty;
}