using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Categories.Application.Interfaces;
using UrlShorter.src.Modules.Categories.Presentation.DTOs;
using UrlShorter.src.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.src.Modules.Categories.Application.UseCases;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> CreateAsync(Guid userId, CategoryDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _categoryRepository.CategoryExistsAsync(c => c.UserId == userId && c.Name == dto.Name, cancellationToken);

        if (exists)
            throw new ConflictException("Category name already exists");

        var category = new Category
        {
            UserId = userId,
            Name = dto.Name
        };

        await _categoryRepository.CreateCategory(category, cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Category created successfully"
        };
    }
}