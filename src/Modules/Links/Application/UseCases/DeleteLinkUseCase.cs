using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Redis;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Links.Application.Interfaces;

namespace UrlShorter.src.Modules.Links.Application.UseCases;

public class DeleteLinkUseCase
{
    private readonly ILinkRepository _linkRepository;
    private readonly IRedisClient _redis;

    public DeleteLinkUseCase(ILinkRepository linkRepository, IRedisClient redis)
    {
        _linkRepository = linkRepository;
        _redis = redis;
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid linkId, CancellationToken cancellationToken = default)
    {
        var link = await _linkRepository.GetFirstOrDefaultLinkAsync(l => l.LinkId == linkId && l.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Link not found");

        await _linkRepository.RemoveLinkAsync(link, cancellationToken);
        await _linkRepository.SaveChangesAsync(cancellationToken);

        var indexKey = $"links:index:{userId}";

        var keys = await _redis.GetSetMembersAsync(indexKey);

        await _redis.DeleteManyAsync(keys);

        await _redis.RemoveSetAsync(indexKey);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Link deleted successfully"
        };
    }

}