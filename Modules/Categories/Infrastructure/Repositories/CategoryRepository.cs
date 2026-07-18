using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UrlShorter.Data;
using UrlShorter.Modules.Categories.Application.Interfaces;
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
}