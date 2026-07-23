
using System.Linq.Expressions;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Links.Application.Dtos;
using UrlShorter.Modules.Links.Application.Queries;
using UrlShorter.Modules.Links.Infrastructure.Models;

namespace UrlShorter.Modules.Links.Application.Interfaces;


public interface ILinkRepository
{
    Task<PagedResult<LinkListDto>> GetLinks(LinkFilter filter, CancellationToken cancellationToken = default);
    Task<bool> ExistsByQueryAsync(Expression<Func<Link, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Link> AddLinkAsync(Link link, CancellationToken cancellationToken = default);
    Task<Link?> GetFirstOrDefaultLinkAsync(Expression<Func<Link, bool>> predicate, CancellationToken cancellationToken = default);
    Task<LinkListDto?> GetLinkAsync(Guid linkId, Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RemoveLinkAsync(Link link, CancellationToken cancellationToken = default);
}
