using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Links.DTOs;

namespace UrlShorter.Modules.Links;

[ApiController]
[Route("api/v1/link")]
[Authorize]
public class LinkController : ControllerBase
{
    private readonly ILinkService _service;

    public LinkController(ILinkService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;
        return Guid.Parse(userId!);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] Guid? categoryId, CreateLinkDto dto)
    {
        var result = await _service.CreateAsync(GetUserId(), categoryId, dto);
        return Ok(result);
    }

    [HttpPatch("{linkId}")]
    public async Task<IActionResult> Update(Guid linkId, UpdateLinkDto dto)
    {
        var result = await _service.UpdateAsync(GetUserId(), linkId, dto);
        return Ok(result);
    }

    [HttpDelete("{linkId}")]
    public async Task<IActionResult> Delete(Guid linkId)
    {
        var result = await _service.DeleteAsync(GetUserId(), linkId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query)
    {
        var result = await _service.GetAllAsync(GetUserId(), query);
        return Ok(result);
    }

    [HttpGet("{linkId}")]
    public async Task<IActionResult> GetById(Guid linkId)
    {
        var result = await _service.GetByIdAsync(GetUserId(), linkId);
        return Ok(result);
    }
}