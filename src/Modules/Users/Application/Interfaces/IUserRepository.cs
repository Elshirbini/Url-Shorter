using System.Linq.Expressions;
using UrlShorter.src.Modules.Users.Infrastructure.Models;

namespace UrlShorter.src.Modules.Users.Application.Interfaces;


public interface IUserRepository
{
    Task<User> SaveUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserDocByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
    Task<User?> GetFirstOrDefaultUserAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
    Task SaveUserChangesAsync(CancellationToken cancellationToken = default);
}