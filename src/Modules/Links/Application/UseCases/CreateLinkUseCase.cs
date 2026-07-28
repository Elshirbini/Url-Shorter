using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Common.Security;
using UrlShorter.src.Modules.Categories.Application.Interfaces;
using UrlShorter.src.Modules.Links.Application.Interfaces;
using UrlShorter.src.Modules.Links.Presentation.DTOs;
using UrlShorter.src.Modules.Links.Infrastructure.Models;

namespace UrlShorter.src.Modules.Links.Application.UseCases;

public class CreateLinkUseCase
{
    private readonly ILinkRepository _linkRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateLinkUseCase(ILinkRepository linkRepository, ICategoryRepository categoryRepository)
    {
        _linkRepository = linkRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> CreateAsync(Guid userId, Guid? categoryId, CreateLinkDto dto, CancellationToken cancellationToken = default)
    {
        if (categoryId.HasValue)
        {
            var categoryExists = await _categoryRepository.CategoryExistsAsync(c => c.CategoryId == categoryId && c.UserId == userId, cancellationToken);

            if (!categoryExists)
                throw new BadRequestException("Invalid category");
        }

        string code;

        if (!string.IsNullOrEmpty(dto.Code))
        {
            code = dto.Code;

            var exists = await _linkRepository.ExistsByQueryAsync(l => l.Code == code, cancellationToken);
            if (exists)
                throw new ConflictException("Code already exists");
        }
        else
        {
            do
            {
                code = CodeGenerator.Generate(6);
            }
            while (await _linkRepository.ExistsByQueryAsync(l => l.Code == code, cancellationToken));
        }

        var link = new Link
        {
            Code = code,
            RedirectUrl = dto.RedirectUrl,
            CategoryId = categoryId,
            UserId = userId
        };

        await _linkRepository.AddLinkAsync(link, cancellationToken);
        await _linkRepository.SaveChangesAsync(cancellationToken);

        return new ApiResponse<object>
        {
            Success = true,
            Data = new
            {
                link.LinkId,
                link.Code
            }
        };
    }
}