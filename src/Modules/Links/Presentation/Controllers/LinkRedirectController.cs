using Microsoft.AspNetCore.Mvc;
using UrlShorter.src.Modules.Links;
using UrlShorter.src.Modules.Links.Application.UseCases;

namespace UrlShorter.src.Modules.Links.Presentation.Controllers;

[ApiController]
public class LinkRedirectController : ControllerBase
{
    private readonly RedirectLinkUseCase _redirectLinkUseCase;

    public LinkRedirectController(RedirectLinkUseCase redirectLinkUseCase)
    {
        _redirectLinkUseCase = redirectLinkUseCase;
    }

    //  PUBLIC ROUTE
    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToUrl(string code, CancellationToken cancellationToken)
    {
        var result = await _redirectLinkUseCase.RedirectAsync(code, HttpContext, cancellationToken);

        return Redirect(result);
    }
}