using UrlShorter.src.Data;
using UrlShorter.src.Modules.Users.Application.Interfaces;
using UrlShorter.src.Modules.Users.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UrlShorter.src.Modules.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> SaveUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var result = await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<User?> GetUserDocByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> UserExistsAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _db.Users.AnyAsync(predicate, cancellationToken);
    }

    public async Task<User?> GetFirstOrDefaultUserAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task SaveUserChangesAsync(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}