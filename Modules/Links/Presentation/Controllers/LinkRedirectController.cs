using Microsoft.AspNetCore.Mvc;
using UrlShorter.Modules.Links;
using UrlShorter.Modules.Links.Application.UseCases;

namespace UrlShorter.Modules.Links.Presentation.Controllers;

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