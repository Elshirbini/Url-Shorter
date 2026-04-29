using UrlShorter.Common.DTOs;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Links.DTOs;

namespace UrlShorter.Modules.Links;

public interface ILinkService
{
    Task<ApiResponse<object>> CreateAsync(Guid userId, Guid? categoryId, CreateLinkDto dto);
    Task<ApiResponse<object>> UpdateAsync(Guid userId, Guid linkId, UpdateLinkDto dto);
    Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid linkId);
    Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query);
    Task<ApiResponse<object>> GetByIdAsync(Guid userId, Guid linkId);
}