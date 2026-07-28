using System.Linq.Expressions;
using UrlShorter.src.Common.DTOs;
using UrlShorter.src.Modules.Categories.Application.Dtos;
using UrlShorter.src.Modules.Categories.Application.Queries;
using UrlShorter.src.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.src.Modules.Categories.Application.Interfaces;

public interface ICategoryRepository
{
    Task<bool> CategoryExistsAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default);
    Task<PagedResult<CategoryListDto>> GetAllCategoriesAsync(CategoryFilter filter, CancellationToken cancellationToken = default);
    Task<Category?> GetFirstOrDefaultCategoriesAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Category> CreateCategory(Category category, CancellationToken cancellationToken = default);
    Task SaveCategoryChanges(CancellationToken cancellationToken = default);
    Task RemoveCategory(Category category, CancellationToken cancellationToken = default);
}