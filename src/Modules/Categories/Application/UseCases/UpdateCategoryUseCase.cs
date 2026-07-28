using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Categories.Application.Interfaces;
using UrlShorter.src.Modules.Categories.Presentation.DTOs;

namespace UrlShorter.src.Modules.Categories.Application.UseCases;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> UpdateAsync(Guid userId, Guid categoryId, CategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetFirstOrDefaultCategoriesAsync(c => c.CategoryId == categoryId && c.UserId == userId, cancellationToken) ?? throw new NotFoundException("Category not found");

        var exists = await _categoryRepository.CategoryExistsAsync(c => c.UserId == userId && c.Name == dto.Name && c.CategoryId != categoryId, cancellationToken);

        if (exists)
            throw new ConflictException("Category name already exists");

        category.Name = dto.Name;

        await _categoryRepository.SaveCategoryChanges(cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Category updated successfully"
        };
    }
}