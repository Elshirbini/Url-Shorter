using UrlShorter.Common.DTOs;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Categories.DTOs;

namespace UrlShorter.Modules.Categories;

public interface ICategoryService
{
    Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query);
    Task<ApiResponse<object>> CreateAsync(Guid userId, CategoryDto dto);
    Task<ApiResponse<object>> UpdateAsync(Guid userId, Guid categoryId, CategoryDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid categoryId);
}