using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Security;
using UrlShorter.Modules.Categories.Application.Interfaces;
using UrlShorter.Modules.Links.Application.Interfaces;
using UrlShorter.Modules.Links.DTOs;
using UrlShorter.Modules.Links.Infrastructure.Models;

namespace UrlShorter.Modules.Links.Application.UseCases;

public class CreateLinkUseCase
{
    private readonly ILinkRepository _linkRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateLinkUseCase(ILinkRepository linkRepository, ICategoryRepository categoryRepository)
    {
        _linkRepository = linkRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<object>> CreateAsync(Guid userId, Guid? categoryId, CreateLinkDto dto)
    {
        if (categoryId.HasValue)
        {
            var categoryExists = await _categoryRepository.CategoryExistsAsync(c => c.CategoryId == categoryId && c.UserId == userId);

            if (!categoryExists)
                throw new BadRequestException("Invalid category");
        }

        string code;

        if (!string.IsNullOrEmpty(dto.Code))
        {
            code = dto.Code;

            var exists = await _linkRepository.ExistsByQueryAsync(l => l.Code == code);
            if (exists)
                throw new ConflictException("Code already exists");
        }
        else
        {
            do
            {
                code = CodeGenerator.Generate(6);
            }
            while (await _linkRepository.ExistsByQueryAsync(l => l.Code == code));
        }

        var link = new Link
        {
            Code = code,
            RedirectUrl = dto.RedirectUrl,
            CategoryId = categoryId,
            UserId = userId
        };

        await _linkRepository.AddLinkAsync(link);
        await _linkRepository.SaveChangesAsync();

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