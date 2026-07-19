using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UrlShorter.Common.DTOs;
using UrlShorter.Data;
using UrlShorter.Modules.Categories.Application.Dtos;
using UrlShorter.Modules.Categories.Application.Interfaces;
using UrlShorter.Modules.Categories.Application.Queries;
using UrlShorter.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.Modules.Categories.Infrastructure.Repositories;

class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }


    public async Task<bool> CategoryExistsAsync(Expression<Func<Category, bool>> predicate)
    {
        return await _db.Categories.AnyAsync(predicate);
    }

    public async Task<PagedResult<CategoryListDto>> GetAllCategoriesAsync(CategoryFilter filter)
    {
        var categoriesQuery = _db.Categories
        .Where(c => c.UserId == filter.UserId);

        var totalCount = await categoriesQuery.CountAsync();

        var data = await categoriesQuery.OrderByDescending(c => c.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(c => new CategoryListDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                // LinksCount = 0 
            })
            .ToListAsync();

        return new PagedResult<CategoryListDto> { Items = data, TotalCount = totalCount };
    }

    public async Task<Category?> GetFirstOrDefaultCategoriesAsync(Expression<Func<Category, bool>> predicate)
    {
        return await _db.Categories.FirstOrDefaultAsync(predicate);
    }

    public async Task<Category> CreateCategory(Category category)
    {
        var result = await _db.Categories.AddAsync(category);
        await _db.SaveChangesAsync();
        return result.Entity;
    }

    public async Task SaveCategoryChanges()
    {
        await _db.SaveChangesAsync();
    }

    public async Task RemoveCategory(Category category)
    {
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }
}