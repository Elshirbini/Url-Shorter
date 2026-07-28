using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UrlShorter.src.Common.DTOs;
using UrlShorter.src.Data;
using UrlShorter.src.Modules.Links.Application.Dtos;
using UrlShorter.src.Modules.Links.Application.Interfaces;
using UrlShorter.src.Modules.Links.Application.Queries;
using UrlShorter.src.Modules.Links.Infrastructure.Models;

namespace UrlShorter.src.Modules.Links.Infrastructure.Repositories;

public class LinkRepository : ILinkRepository
{
    private readonly AppDbContext _db;

    public LinkRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<LinkListDto>> GetLinks(LinkFilter filter, CancellationToken cancellationToken = default)
    {

        var linksQuery = _db.Links
            .Where(l => l.UserId == filter.UserId);

        //  filter by category
        if (filter.CategoryId.HasValue)
        {
            linksQuery = linksQuery
                .Where(l => l.CategoryId == filter.CategoryId);
        }

        //  search (code + redirectUrl)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();

            linksQuery = linksQuery.Where(l =>
                l.Code.Contains(search) ||
                l.RedirectUrl.Contains(search)
            );
        }

        var total = await linksQuery.CountAsync(cancellationToken);

        var data = await linksQuery
            .OrderByDescending(l => l.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new LinkListDto
            {
                LinkId = l.LinkId,
                Code = l.Code,
                RedirectUrl = l.RedirectUrl,
                Clicks = l.Clicks,
                CreatedAt = l.CreatedAt,
                CategoryId = l.CategoryId
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<LinkListDto>
        {
            Items = data,
            TotalCount = total
        };
    }

    public async Task<LinkListDto?> GetLinkAsync(Guid linkId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Links
            .Where(l => l.LinkId == linkId && l.UserId == userId)
            .Select(l => new LinkListDto
            {
                LinkId = l.LinkId,
                Code = l.Code,
                RedirectUrl = l.RedirectUrl,
                Clicks = l.Clicks,
                CreatedAt = l.CreatedAt,
                CategoryId = l.CategoryId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByQueryAsync(Expression<Func<Link, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _db.Links.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Link> AddLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        var linkEntity = await _db.Links.AddAsync(link, cancellationToken);
        return linkEntity.Entity;
    }

    public async Task<Link?> GetFirstOrDefaultLinkAsync(Expression<Func<Link, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _db.Links.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        _db.Links.Remove(link);
    }
}