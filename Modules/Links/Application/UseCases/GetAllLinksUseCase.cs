using UrlShorter.Common.DTOs;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Links.Application.Interfaces;
using UrlShorter.Modules.Links.Application.Queries;

namespace UrlShorter.Modules.Links.Application.UseCases;

public class GetAllLinksUseCase
{
    private readonly ILinkRepository _linkRepository;

    public GetAllLinksUseCase(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }


    public async Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query)
    {

        var pagedLinks = await _linkRepository.GetLinks(new LinkFilter { UserId = userId, CategoryId = query.CategoryId, Search = query.Search, Page = query.Page, PageSize = query.PageSize });

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