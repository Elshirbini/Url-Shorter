using UrlShorter.Data;
using UrlShorter.Modules.Links.Application.Dtos;
using UrlShorter.Modules.Links.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using UrlShorter.Modules.Links.Infrastructure.Models;
namespace UrlShorter.Modules.Links.Infrastructure.Repositories;

class ClickRepository : IClickRepository
{
    private readonly AppDbContext _db;

    public ClickRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LinkAnalyticsData> GetClickAnalyticsAsync(Guid linkId)
    {
        var clicksQuery = _db.Clicks
            .Where(c => c.LinkId == linkId);


        //  recent clicks
        var recentClicks = await clicksQuery
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new RecentClickDto
            {
                DeviceType = c.DeviceType,
                Referer = c.Referer,
                Ip = c.Ip,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();


        //  device stats
        var deviceStats = await clicksQuery
            .GroupBy(c => c.DeviceType)
            .Select(g => new DeviceStatDto
            {
                Device = g.Key,
                Count = g.Count()
            })
            .ToListAsync();


        //  top referers
        var topReferers = await clicksQuery
            .Where(c => c.Referer != null)
            .GroupBy(c => c.Referer)
            .Select(g => new RefererStatDto
            {
                Referer = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();


        //  unique visitors (by IP)
        var uniqueVisitors = await clicksQuery
            .Where(c => c.Ip != null)
            .Select(c => c.Ip)
            .Distinct()
            .CountAsync();


        // clicks by day (time series)
        var clicksByDay = await clicksQuery
            .GroupBy(c => c.CreatedAt.Date)
            .Select(g => new ClickByDayDto
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();


        // top IPs (abuse detection)
        var topIPs = await clicksQuery
            .Where(c => c.Ip != null)
            .GroupBy(c => c.Ip)
            .Select(g => new TopIpDto
            {
                Ip = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        return new LinkAnalyticsData
        {
            RecentClicks = recentClicks,
            DeviceStats = deviceStats,
            TopReferers = topReferers,
            UniqueVisitors = uniqueVisitors,
            ClicksByDay = clicksByDay,
            TopIps = topIPs
        };
    }


    public async Task<Click> AddClickAsync(Click click)
    {
        var clickEntity = await _db.Clicks.AddAsync(click);
        await _db.SaveChangesAsync();
        return clickEntity.Entity;
    }
}