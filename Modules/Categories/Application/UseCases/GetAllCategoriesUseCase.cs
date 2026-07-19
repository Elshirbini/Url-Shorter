using UrlShorter.Common.DTOs;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Categories.Application.Interfaces;
using UrlShorter.Modules.Categories.Application.Queries;

namespace UrlShorter.Modules.Categories.Application.UseCases;

public class GetAllCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query)
    {
        var result = await _categoryRepository.GetAllCategoriesAsync(new CategoryFilter
        {
            UserId = userId,
            Page = query.Page,
            PageSize = query.PageSize
        });

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