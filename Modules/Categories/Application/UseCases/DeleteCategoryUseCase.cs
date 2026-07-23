using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Categories.Application.Interfaces;

namespace UrlShorter.Modules.Categories.Application.UseCases;

public class DeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetFirstOrDefaultCategoriesAsync(c => c.CategoryId == categoryId && c.UserId == userId, cancellationToken) ?? throw new NotFoundException("Category not found");


        await _categoryRepository.RemoveCategory(category, cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Category deleted successfully"
        };
    }
}