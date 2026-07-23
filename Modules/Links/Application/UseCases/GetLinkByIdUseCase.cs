using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Data;
using UrlShorter.Modules.Links.Application.Interfaces;

namespace UrlShorter.Modules.Links.Application.UseCases;


public class GetLinkByIdUseCase
{
    private readonly ILinkRepository _linkRepository;
    private readonly IClickRepository _clickRepository;

    public GetLinkByIdUseCase(ILinkRepository linkRepository, IClickRepository clickRepository)
    {
        _linkRepository = linkRepository;
        _clickRepository = clickRepository;
    }

    public async Task<ApiResponse<object>> GetByIdAsync(Guid userId, Guid linkId, CancellationToken cancellationToken = default)
    {
        //  get basic link data
        var link = await _linkRepository.GetLinkAsync(linkId, userId, cancellationToken)
            ?? throw new NotFoundException("Link not found");


        //  base query for clicks
        var analytics = await _clickRepository.GetClickAnalyticsAsync(linkId, cancellationToken);


        return new ApiResponse<object>
        {
            Success = true,
            Data = new
            {
                link,
                analytics
            }
        };
    }
}