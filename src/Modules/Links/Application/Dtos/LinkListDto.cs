using UrlShorter.src.Modules.Links.Infrastructure.Models;

namespace UrlShorter.src.Modules.Links.Application.Dtos;

public class LinkListDto
{
    public Guid LinkId { get; set; }
    public string? Code { get; set; }
    public string? RedirectUrl { get; set; }
    public int Clicks { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CategoryId { get; set; }
}