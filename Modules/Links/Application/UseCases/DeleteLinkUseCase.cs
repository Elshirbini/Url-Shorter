using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Links.Application.Interfaces;

namespace UrlShorter.Modules.Links.Application.UseCases;

public class DeleteLinkUseCase
{
    private readonly ILinkRepository _linkRepository;

    public DeleteLinkUseCase(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }

    public async Task<ApiResponse<object>> DeleteAsync(Guid userId, Guid linkId, CancellationToken cancellationToken = default)
    {
        var link = await _linkRepository.GetFirstOrDefaultLinkAsync(l => l.LinkId == linkId && l.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Link not found");

        await _linkRepository.RemoveLinkAsync(link, cancellationToken);
        await _linkRepository.SaveChangesAsync(cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Link deleted successfully"
        };
    }

}