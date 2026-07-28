using UrlShorter.src.Data;
using UrlShorter.src.Modules.Links.Application.Dtos;
using UrlShorter.src.Modules.Links.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using UrlShorter.src.Modules.Links.Infrastructure.Models;
namespace UrlShorter.src.Modules.Links.Infrastructure.Repositories;

class ClickRepository : IClickRepository
{
    private readonly AppDbContext _db;

    public ClickRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LinkAnalyticsData> GetClickAnalyticsAsync(Guid linkId, CancellationToken cancellationToken = default)
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
            .ToListAsync(cancellationToken);


        //  device stats
        var deviceStats = await clicksQuery
            .GroupBy(c => c.DeviceType)
            .Select(g => new DeviceStatDto
            {
                Device = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);


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
            .ToListAsync(cancellationToken);


        //  unique visitors (by IP)
        var uniqueVisitors = await clicksQuery
            .Where(c => c.Ip != null)
            .Select(c => c.Ip)
            .Distinct()
            .CountAsync(cancellationToken);


        // clicks by day (time series)
        var clicksByDay = await clicksQuery
            .GroupBy(c => c.CreatedAt.Date)
            .Select(g => new ClickByDayDto
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);


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
            .ToListAsync(cancellationToken);

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


    public async Task<Click> AddClickAsync(Click click, CancellationToken cancellationToken = default)
    {
        var clickEntity = await _db.Clicks.AddAsync(click, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return clickEntity.Entity;
    }
}