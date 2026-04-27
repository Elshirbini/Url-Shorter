using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Categories.DTOs;

namespace UrlShorter.Modules.Categories;

[ApiController]
[Route("api/v1/category")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;
        return Guid.Parse(userId!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query)
    {
        var result = await _service.GetAllAsync(GetUserId(), query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        var result = await _service.CreateAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpPatch("{categoryId}")]
    public async Task<IActionResult> Update(Guid categoryId, CategoryDto dto)
    {
        var result = await _service.UpdateAsync(GetUserId(), categoryId, dto);
        return Ok(result);
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> Delete(Guid categoryId)
    {
        var result = await _service.DeleteAsync(GetUserId(), categoryId);
        return Ok(result);
    }
}