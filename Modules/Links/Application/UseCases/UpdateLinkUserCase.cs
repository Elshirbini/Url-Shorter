using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Categories.Application.Interfaces;
using UrlShorter.Modules.Links.Application.Interfaces;
using UrlShorter.Modules.Links.Presentation.DTOs;

namespace UrlShorter.Modules.Links.Application.UseCases;


public class UpdateLinkUseCase
{
    private readonly ILinkRepository _linkRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateLinkUseCase(ILinkRepository linkRepository, ICategoryRepository categoryRepository)
    {
        _linkRepository = linkRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> UpdateAsync(Guid userId, Guid linkId, UpdateLinkDto dto, CancellationToken cancellationToken = default)
    {
        var link = await _linkRepository.GetFirstOrDefaultLinkAsync(l => l.LinkId == linkId && l.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Link not found");

        if (!string.IsNullOrEmpty(dto.Code))
        {
            var exists = await _linkRepository
                .ExistsByQueryAsync(l => l.Code == dto.Code && l.LinkId != linkId, cancellationToken);

            if (exists)
                throw new ConflictException("Code already exists");

            link.Code = dto.Code;
        }

        if (!string.IsNullOrEmpty(dto.RedirectUrl))
            link.RedirectUrl = dto.RedirectUrl;

        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _categoryRepository.CategoryExistsAsync(c => c.CategoryId == dto.CategoryId && c.UserId == userId, cancellationToken);

            if (!categoryExists)
                throw new BadRequestException("Invalid category");

            link.CategoryId = dto.CategoryId;
        }

        link.UpdatedAt = DateTime.UtcNow;

        await _linkRepository.SaveChangesAsync(cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Link updated successfully"
        };
    }
}