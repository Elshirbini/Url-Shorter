using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.src.Common.DTOs;
using UrlShorter.src.Modules.Categories.Application.UseCases;
using UrlShorter.src.Modules.Categories.Presentation.DTOs;

namespace UrlShorter.src.Modules.Categories.Presentation;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly CreateCategoryUseCase createCategoryUseCase;
    private readonly UpdateCategoryUseCase updateCategoryUseCase;
    private readonly DeleteCategoryUseCase deleteCategoryUseCase;
    private readonly GetAllCategoriesUseCase getAllCategoriesUseCase;

    public CategoryController(CreateCategoryUseCase createCategoryUseCase, UpdateCategoryUseCase updateCategoryUseCase, DeleteCategoryUseCase deleteCategoryUseCase, GetAllCategoriesUseCase getAllCategoriesUseCase)
    {
        this.createCategoryUseCase = createCategoryUseCase;
        this.updateCategoryUseCase = updateCategoryUseCase;
        this.deleteCategoryUseCase = deleteCategoryUseCase;
        this.getAllCategoriesUseCase = getAllCategoriesUseCase;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;
        return Guid.Parse(userId!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, CancellationToken cancellationToken)
    {
        var result = await getAllCategoriesUseCase.GetAllAsync(GetUserId(), query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto, CancellationToken cancellationToken)
    {
        var result = await createCategoryUseCase.CreateAsync(GetUserId(), dto, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{categoryId}")]
    public async Task<IActionResult> Update(Guid categoryId, CategoryDto dto, CancellationToken cancellationToken)
    {
        var result = await updateCategoryUseCase.UpdateAsync(GetUserId(), categoryId, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await deleteCategoryUseCase.DeleteAsync(GetUserId(), categoryId, cancellationToken);
        return Ok(result);
    }
}