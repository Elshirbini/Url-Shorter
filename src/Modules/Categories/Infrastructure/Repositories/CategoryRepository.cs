using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UrlShorter.src.Common.DTOs;
using UrlShorter.src.Data;
using UrlShorter.src.Modules.Categories.Application.Dtos;
using UrlShorter.src.Modules.Categories.Application.Interfaces;
using UrlShorter.src.Modules.Categories.Application.Queries;
using UrlShorter.src.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.src.Modules.Categories.Infrastructure.Repositories;

class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }


    public async Task<bool> CategoryExistsAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _db.Categories.AnyAsync(predicate, cancellationToken);
    }

    public async Task<PagedResult<CategoryListDto>> GetAllCategoriesAsync(CategoryFilter filter, CancellationToken cancellationToken = default)
    {
        var categoriesQuery = _db.Categories
        .Where(c => c.UserId == filter.UserId);

        var totalCount = await categoriesQuery.CountAsync(cancellationToken);

        var data = await categoriesQuery.OrderByDescending(c => c.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(c => new CategoryListDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                // LinksCount = 0 
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<CategoryListDto> { Items = data, TotalCount = totalCount };
    }

    public async Task<Category?> GetFirstOrDefaultCategoriesAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _db.Categories.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Category> CreateCategory(Category category, CancellationToken cancellationToken = default)
    {
        var result = await _db.Categories.AddAsync(category, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task SaveCategoryChanges(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveCategory(Category category, CancellationToken cancellationToken = default)
    {
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);
    }
}