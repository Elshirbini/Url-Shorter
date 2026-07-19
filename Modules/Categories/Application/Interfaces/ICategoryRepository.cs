using System.Linq.Expressions;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Categories.Application.Dtos;
using UrlShorter.Modules.Categories.Application.Queries;
using UrlShorter.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.Modules.Categories.Application.Interfaces;

public interface ICategoryRepository
{
    Task<bool> CategoryExistsAsync(Expression<Func<Category, bool>> predicate);
    Task<PagedResult<CategoryListDto>> GetAllCategoriesAsync(CategoryFilter filter);
    Task<Category?> GetFirstOrDefaultCategoriesAsync(Expression<Func<Category, bool>> predicate);
    Task<Category> CreateCategory(Category category);
    Task SaveCategoryChanges();
    Task RemoveCategory(Category category);
}