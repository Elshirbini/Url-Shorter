using System;

namespace UrlShorter.src.Modules.Links.Presentation.DTOs;

public class UpdateLinkDto
{
    public string? Code { get; set; }

    public string? RedirectUrl { get; set; }

    public Guid? CategoryId { get; set; }
}