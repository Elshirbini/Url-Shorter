
using System.Linq.Expressions;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Links.Application.Dtos;
using UrlShorter.Modules.Links.Application.Queries;
using UrlShorter.Modules.Links.Infrastructure.Models;

namespace UrlShorter.Modules.Links.Application.Interfaces;


public interface ILinkRepository
{
    Task<PagedResult<LinkListDto>> GetLinks(LinkFilter filter);
    Task<bool> ExistsByQueryAsync(Expression<Func<Link, bool>> predicate);
    Task<Link> AddLinkAsync(Link link);
    Task<Link?> GetFirstOrDefaultLinkAsync(Expression<Func<Link, bool>> predicate);
    Task<LinkListDto?> GetLinkAsync(Guid linkId, Guid userId);
    Task SaveChangesAsync();
    Task RemoveLinkAsync(Link link);
}
