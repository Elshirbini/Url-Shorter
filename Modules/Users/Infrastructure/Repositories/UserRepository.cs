using UrlShorter.Data;
using UrlShorter.Modules.Users.Application.Interfaces;
using UrlShorter.Modules.Users.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UrlShorter.Modules.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> SaveUserAsync(User user)
    {
        var result = await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new User
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task<User?> GetUserDocByIdAsync(Guid userId)
    {
        return await _db.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UserExistsAsync(Expression<Func<User, bool>> predicate)
    {
        return await _db.Users.AnyAsync(predicate);
    }

    public async Task<User?> GetFirstOrDefaultUserAsync(Expression<Func<User, bool>> predicate)
    {
        return await _db.Users.FirstOrDefaultAsync(predicate);
    }

    public async Task SaveUserChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}