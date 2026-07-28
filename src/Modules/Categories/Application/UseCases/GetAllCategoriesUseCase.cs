using UrlShorter.src.Common.DTOs;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Categories.Application.Interfaces;
using UrlShorter.src.Modules.Categories.Application.Queries;

namespace UrlShorter.src.Modules.Categories.Application.UseCases;

public class GetAllCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query, CancellationToken cancellationToken = default)
    {
        var result = await _categoryRepository.GetAllCategoriesAsync(new CategoryFilter
        {
            UserId = userId,
            Page = query.Page,
            PageSize = query.PageSize
        }, cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Data = result.Items,
            Meta = new
            {
                totalCount = result.TotalCount,
                page = query.Page,
            }
        };
    }
}