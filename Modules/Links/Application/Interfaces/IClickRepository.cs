using UrlShorter.Modules.Links.Application.Dtos;
using UrlShorter.Modules.Links.Infrastructure.Models;

namespace UrlShorter.Modules.Links.Application.Interfaces;


public interface IClickRepository
{
    Task<LinkAnalyticsData> GetClickAnalyticsAsync(Guid linkId, CancellationToken cancellationToken = default);
    Task<Click> AddClickAsync(Click click, CancellationToken cancellationToken = default);
}