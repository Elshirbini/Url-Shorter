namespace UrlShorter.src.Modules.Links.Application.Dtos;

public class LinkAnalyticsData
{
    public List<RecentClickDto>? RecentClicks { get; set; }

    public List<DeviceStatDto>? DeviceStats { get; set; }

    public List<RefererStatDto>? TopReferers { get; set; }

    public int UniqueVisitors { get; set; }

    public List<ClickByDayDto>? ClicksByDay { get; set; }

    public List<TopIpDto>? TopIps { get; set; }
}