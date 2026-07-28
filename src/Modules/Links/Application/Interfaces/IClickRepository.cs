using UrlShorter.src.Modules.Links.Application.Dtos;
using UrlShorter.src.Modules.Links.Infrastructure.Models;

namespace UrlShorter.src.Modules.Links.Application.Interfaces;


public interface IClickRepository
{
    Task<LinkAnalyticsData> GetClickAnalyticsAsync(Guid linkId, CancellationToken cancellationToken = default);
    Task<Click> AddClickAsync(Click click, CancellationToken cancellationToken = default);
}