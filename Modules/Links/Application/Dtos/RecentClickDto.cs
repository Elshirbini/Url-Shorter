namespace UrlShorter.Modules.Links.Application.Dtos;

public class RecentClickDto
{
    public string? DeviceType { get; set; }
    public string? Referer { get; set; }
    public string? Ip { get; set; }
    public DateTime CreatedAt { get; set; }
}