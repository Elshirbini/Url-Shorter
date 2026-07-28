using UrlShorter.src.Modules.Auth.Application.Interfaces;

namespace UrlShorter.src.Modules.Auth.Jobs;

public class CleanupRefreshTokensJob
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<CleanupRefreshTokensJob> _logger;

    public CleanupRefreshTokensJob(
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<CleanupRefreshTokensJob> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        int deleted = await _refreshTokenRepository
            .DeleteExpiredRefreshTokensAsync(cancellationToken);

        _logger.LogInformation(
            "Deleted {Count} expired refresh tokens.",
            deleted);
    }
}