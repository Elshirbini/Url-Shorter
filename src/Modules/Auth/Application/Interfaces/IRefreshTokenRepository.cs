using System.Linq.Expressions;
using UrlShorter.src.Modules.Auth.Infrastructure.Models;

namespace UrlShorter.src.Modules.Auth.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetFirstOrDefaultRefreshTokenAsync(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default);
    Task<RefreshToken> SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<int> DeleteExpiredRefreshTokensAsync(CancellationToken cancellationToken = default);
}