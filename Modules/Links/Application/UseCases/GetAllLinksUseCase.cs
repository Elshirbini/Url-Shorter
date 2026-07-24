using System.Text.Json;
using UrlShorter.Common.DTOs;
using UrlShorter.Common.Redis;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Links.Application.Dtos;
using UrlShorter.Modules.Links.Application.Interfaces;
using UrlShorter.Modules.Links.Application.Queries;

namespace UrlShorter.Modules.Links.Application.UseCases;

public class GetAllLinksUseCase
{
    private readonly ILinkRepository _linkRepository;
    private readonly IRedisClient _redis;
    private readonly ILogger<GetAllLinksUseCase> _logger;

    public GetAllLinksUseCase(ILinkRepository linkRepository, IRedisClient redis, ILogger<GetAllLinksUseCase> logger)
    {
        _linkRepository = linkRepository;
        _redis = redis;
        _logger = logger;
    }


    public async Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query, CancellationToken cancellationToken = default)
    {

        var cacheKey = $"links:{userId}:{query.CategoryId}:{query.Search}:{query.Page}:{query.PageSize}";

        var cached = await _redis.GetAsync<PagedResult<LinkListDto>>(cacheKey);

        if (cached != null)
        {

            return new ApiResponse<object>
            {
                Success = true,
                Data = cached.Items,
                Meta = new
                {
                    total = cached.TotalCount,
                    page = query.Page,
                    pageSize = query.PageSize
                }
            };
        }
        else
        {

            var pagedLinks = await _linkRepository.GetLinks(new LinkFilter { UserId = userId, CategoryId = query.CategoryId, Search = query.Search, Page = query.Page, PageSize = query.PageSize }, cancellationToken);

            await _redis.SetAsync<PagedResult<LinkListDto>>(cacheKey, pagedLinks, TimeSpan.FromMinutes(10));

            return new ApiResponse<object>
            {
                Success = true,
                Data = pagedLinks.Items,
                Meta = new
                {
                    total = pagedLinks.TotalCount,
                    page = query.Page,
                    pageSize = query.PageSize
                }
            };
        }

    }
}