namespace UrlShorter.Modules.Links.Presentation.DTOs;

public class CreateLinkDto
{
    public string? Code { get; set; } // optional (custom)

    public string RedirectUrl { get; set; } = string.Empty;
}