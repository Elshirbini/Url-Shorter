using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Links.DTOs;

public class UpdateLinkDto
{
    [StringLength(6, ErrorMessage = "Code must be 6 characters")]
    public string? Code { get; set; }

    [Url(ErrorMessage = "Invalid URL")]
    public string? RedirectUrl { get; set; }

    public Guid? CategoryId { get; set; }
}