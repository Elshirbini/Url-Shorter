using System.Linq.Expressions;
using UrlShorter.Modules.Users.Infrastructure.Models;

namespace UrlShorter.Modules.Users.Application.Interfaces;


public interface IUserRepository
{
    Task<User> SaveUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserDocByIdAsync(Guid userId);
    Task<bool> UserExistsAsync(Expression<Func<User, bool>> predicate);
    Task<User?> GetFirstOrDefaultUserAsync(Expression<Func<User, bool>> predicate);
    Task SaveUserChangesAsync();
}