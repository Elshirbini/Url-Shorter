using Microsoft.EntityFrameworkCore;
using UrlShorter.Common.DTOs;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Data;
using UrlShorter.Modules.Categories.DTOs;
using UrlShorter.Modules.Categories.Models;

namespace UrlShorter.Modules.Categories;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<object>> GetAllAsync(Guid userId, QueryParams query)
    {
        var categoriesQuery = _db.Categories
        .Where(c => c.UserId == userId);

        var totalCount = await categoriesQuery.CountAsync();

        var data = await categoriesQuery.OrderByDescending(c => c.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new
            {
                c.CategoryId,
                c.Name,
                // LinksCount = 0 
            })
            .ToListAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Data = data,
            Meta = new
            {
                totalCount,
                page = query.Page,
            }
        };
    }

    public async Task<ApiResponse<object>> CreateAsync(Guid userId, CategoryDto dto)
    {
        var exists = await _db.Categories
            .AnyAsync(c => c.UserId == userId && c.Name == dto.Name);

        if (exists)
            throw new ConflictException("Category name already exists");

        var category = new Category
        {
            UserId = userId,
            Name = dto.Name
        };

        await _db.Categories.AddAsync(category);
        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Category created successfully"
        };
    }

    public async Task<ApiResponse<object>> UpdateAsync(Guid userId, Guid categoryId, CategoryDto dto)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.UserId == userId) ?? throw new NotFoundException("Category not found");

        var exists = await _db.Categories
            .AnyAsync(c => c.UserId == userId && c.Name == dto.Name && c.CategoryId != categoryId);

        if (exists)
            throw new ConflictException("Category name already exists");

        category.Name = dto.Name;

        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Category updated successfully"
        };
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid categoryId)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.UserId == userId) ?? throw new NotFoundException("Category not found");


        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Category deleted successfully"
        };
    }
}