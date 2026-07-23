using System.Linq.Expressions;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Categories.Application.Dtos;
using UrlShorter.Modules.Categories.Application.Queries;
using UrlShorter.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.Modules.Categories.Application.Interfaces;

public interface ICategoryRepository
{
    Task<bool> CategoryExistsAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default);
    Task<PagedResult<CategoryListDto>> GetAllCategoriesAsync(CategoryFilter filter, CancellationToken cancellationToken = default);
    Task<Category?> GetFirstOrDefaultCategoriesAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Category> CreateCategory(Category category, CancellationToken cancellationToken = default);
    Task SaveCategoryChanges(CancellationToken cancellationToken = default);
    Task RemoveCategory(Category category, CancellationToken cancellationToken = default);
}