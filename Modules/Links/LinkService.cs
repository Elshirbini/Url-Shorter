using Microsoft.EntityFrameworkCore;
using UrlShorter.Common.DTOs;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Security;
using UrlShorter.Data;
using UrlShorter.Modules.Links.DTOs;
using UrlShorter.Modules.Links.Models;

namespace UrlShorter.Modules.Links;

public class LinkService : ILinkService
{
    private readonly AppDbContext _db;

    public LinkService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<object>> CreateAsync(Guid userId, Guid? categoryId, CreateLinkDto dto)
    {
        if (categoryId.HasValue)
        {
            var categoryExists = await _db.Categories
                .AnyAsync(c => c.CategoryId == categoryId && c.UserId == userId);

            if (!categoryExists)
                throw new BadRequestException("Invalid category");
        }

        string code;

        if (!string.IsNullOrEmpty(dto.Code))
        {
            code = dto.Code;

            var exists = await _db.Links.AnyAsync(l => l.Code == code);
            if (exists)
                throw new ConflictException("Code already exists");
        }
        else
        {
            do
            {
                code = CodeGenerator.Generate(6);
            }
            while (await _db.Links.AnyAsync(l => l.Code == code));
        }

        var link = new Link
        {
            Code = code,
            RedirectUrl = dto.RedirectUrl,
            CategoryId = categoryId,
            UserId = userId
        };

        await _db.Links.AddAsync(link);
        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Data = new
            {
                link.LinkId,
                link.Code
            }
        };
    }

    public async Task<ApiResponse<object>> UpdateAsync(Guid userId, Guid linkId, UpdateLinkDto dto)
    {
        var link = await _db.Links
            .FirstOrDefaultAsync(l => l.LinkId == linkId && l.UserId == userId)
            ?? throw new NotFoundException("Link not found");

        if (!string.IsNullOrEmpty(dto.Code))
        {
            var exists = await _db.Links
                .AnyAsync(l => l.Code == dto.Code && l.LinkId != linkId);

            if (exists)
                throw new ConflictException("Code already exists");

            link.Code = dto.Code;
        }

        if (!string.IsNullOrEmpty(dto.RedirectUrl))
            link.RedirectUrl = dto.RedirectUrl;

        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _db.Categories
                .AnyAsync(c => c.CategoryId == dto.CategoryId && c.UserId == userId);

            if (!categoryExists)
                throw new BadRequestException("Invalid category");

            link.CategoryId = dto.CategoryId;
        }

        link.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Link updated successfully"
        };
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid linkId)
    {
        var link = await _db.Links
            .FirstOrDefaultAsync(l => l.LinkId == linkId && l.UserId == userId)
            ?? throw new NotFoundException("Link not found");

        _db.Links.Remove(link);
        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Link deleted successfully"
        };
    }

    public async Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query)
    {
        var linksQuery = _db.Links
            .Where(l => l.UserId == userId);

        //  filter by category
        if (query.CategoryId.HasValue)
        {
            linksQuery = linksQuery
                .Where(l => l.CategoryId == query.CategoryId);
        }

        //  search (code + redirectUrl)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            linksQuery = linksQuery.Where(l =>
                l.Code.Contains(search) ||
                l.RedirectUrl.Contains(search)
            );
        }

        var total = await linksQuery.CountAsync();

        var data = await linksQuery
            .OrderByDescending(l => l.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(l => new
            {
                l.LinkId,
                l.Code,
                l.RedirectUrl,
                l.Clicks,
                l.CreatedAt,
                l.CategoryId
            })
            .ToListAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Data = data,
            Meta = new
            {
                total,
                page = query.Page,
                pageSize = query.PageSize
            }
        };
    }

    public async Task<ApiResponse<object>> GetByIdAsync(Guid userId, Guid linkId)
    {
        //  get basic link data
        var link = await _db.Links
            .Where(l => l.LinkId == linkId && l.UserId == userId)
            .Select(l => new
            {
                l.LinkId,
                l.Code,
                l.RedirectUrl,
                l.Clicks,
                l.CreatedAt
            })
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Link not found");


        //  base query for clicks
        var clicksQuery = _db.Clicks
            .Where(c => c.LinkId == linkId);


        //  recent clicks
        var recentClicks = await clicksQuery
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new
            {
                c.DeviceType,
                c.Referer,
                c.Ip,
                c.CreatedAt
            })
            .ToListAsync();


        //  device stats
        var deviceStats = await clicksQuery
            .GroupBy(c => c.DeviceType)
            .Select(g => new
            {
                Device = g.Key,
                Count = g.Count()
            })
            .ToListAsync();


        //  top referers
        var topReferers = await clicksQuery
            .Where(c => c.Referer != null)
            .GroupBy(c => c.Referer)
            .Select(g => new
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
            .Select(g => new
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
            .Select(g => new
            {
                IP = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();


        return new ApiResponse<object>
        {
            Success = true,
            Data = new
            {
                link,
                analytics = new
                {
                    recentClicks,
                    deviceStats,
                    topReferers,
                    uniqueVisitors,
                    clicksByDay,
                    topIPs
                }
            }
        };
    }
}