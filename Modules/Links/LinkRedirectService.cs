using Microsoft.EntityFrameworkCore;
using UrlShorter.Common.Exceptions;
using UrlShorter.Data;
using UrlShorter.Modules.Links.Models;

namespace UrlShorter.Modules.Links;

public class LinkRedirectService : ILinkRedirectService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ILinkRedirectService> _logger;

    public LinkRedirectService(AppDbContext db , ILogger<ILinkRedirectService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<string> RedirectAsync(string code, HttpContext context)
    {
        var link = await _db.Links
            .FirstOrDefaultAsync(l => l.Code == code)
            ?? throw new NotFoundException("Link not found");

        var userAgent = context.Request.Headers.UserAgent.ToString();
        var referer = context.Request.Headers.Referer.ToString();
        var ip = context.Connection.RemoteIpAddress?.ToString();

        var device = DetectDevice(userAgent);

        var click = new Click
        {
            LinkId = link.LinkId,
            DeviceType = device,
            Referer = string.IsNullOrWhiteSpace(referer) ? "direct" : referer,
            Ip = string.IsNullOrWhiteSpace(ip) ? "unknown" : ip
        };

        await _db.Clicks.AddAsync(click);

        link.Clicks += 1;

        await _db.SaveChangesAsync();

        return link.RedirectUrl;
    }

    private string DetectDevice(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "unknown";

        userAgent = userAgent.ToLower();

        if (userAgent.Contains("mobile"))
            return "mobile";

        if (userAgent.Contains("tablet"))
            return "tablet";

        return "desktop";
    }
}