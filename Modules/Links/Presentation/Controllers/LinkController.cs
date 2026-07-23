using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UrlShorter.Common.DTOs;
using UrlShorter.Modules.Links.Application.UseCases;
using UrlShorter.Modules.Links.Presentation.DTOs;

namespace UrlShorter.Modules.Links.Presentation.Controllers;

[ApiController]
[Route("api/v1/link")]
[Authorize]
public class LinkController : ControllerBase
{
    private readonly CreateLinkUseCase _createLinkUseCase;
    private readonly UpdateLinkUseCase _updateLinkUseCase;
    private readonly DeleteLinkUseCase _deleteLinkUseCase;
    private readonly GetAllLinksUseCase _getAllLinksUseCase;
    private readonly GetLinkByIdUseCase _getLinkByIdUseCase;

    public LinkController(CreateLinkUseCase createLinkUseCase, UpdateLinkUseCase updateLinkUseCase, DeleteLinkUseCase deleteLinkUseCase, GetAllLinksUseCase getAllLinksUseCase, GetLinkByIdUseCase getLinkByIdUseCase)
    {
        _createLinkUseCase = createLinkUseCase;
        _updateLinkUseCase = updateLinkUseCase;
        _deleteLinkUseCase = deleteLinkUseCase;
        _getAllLinksUseCase = getAllLinksUseCase;
        _getLinkByIdUseCase = getLinkByIdUseCase;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirst("userId")?.Value;
        return Guid.Parse(userId!);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromQuery] Guid? categoryId, CreateLinkDto dto, CancellationToken cancellationToken)
    {
        var result = await _createLinkUseCase.CreateAsync(GetUserId(), categoryId, dto, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{linkId}")]
    public async Task<IActionResult> Update(Guid linkId, UpdateLinkDto dto, CancellationToken cancellationToken)
    {
        var result = await _updateLinkUseCase.UpdateAsync(GetUserId(), linkId, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{linkId}")]
    public async Task<IActionResult> Delete(Guid linkId, CancellationToken cancellationToken)
    {
        var result = await _deleteLinkUseCase.DeleteAsync(GetUserId(), linkId, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, CancellationToken cancellationToken)
    {
        var result = await _getAllLinksUseCase.GetAllAsync(GetUserId(), query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{linkId}")]
    public async Task<IActionResult> GetById(Guid linkId, CancellationToken cancellationToken)
    {
        var result = await _getLinkByIdUseCase.GetByIdAsync(GetUserId(), linkId, cancellationToken);
        return Ok(result);
    }
}