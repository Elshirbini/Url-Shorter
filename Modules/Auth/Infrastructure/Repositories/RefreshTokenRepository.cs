using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UrlShorter.Data;
using UrlShorter.Modules.Auth.Application.Interfaces;
using UrlShorter.Modules.Auth.Infrastructure.Models;

namespace UrlShorter.Modules.Auth.Infrastructure.Repositories;


public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RefreshToken> SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        var result = await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<RefreshToken?> GetFirstOrDefaultRefreshTokenAsync(Expression<Func<RefreshToken, bool>> predicate)
    {
        return await _db.RefreshTokens.FirstOrDefaultAsync(predicate);
    }
}