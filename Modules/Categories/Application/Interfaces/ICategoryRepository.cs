using System.Linq.Expressions;
using UrlShorter.Modules.Categories.Infrastructure.Models;

namespace UrlShorter.Modules.Categories.Application.Interfaces;

public interface ICategoryRepository
{
    Task<bool> CategoryExistsAsync(Expression<Func<Category, bool>> predicate);
}