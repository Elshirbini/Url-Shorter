using Microsoft.AspNetCore.Mvc;
using UrlShorter.Modules.Links;

namespace UrlShorter.Modules.Links;

[ApiController]
public class LinkRedirectController : ControllerBase
{
    private readonly ILinkRedirectService _service;

    public LinkRedirectController(ILinkRedirectService service)
    {
        _service = service;
    }

    // 🔥 PUBLIC ROUTE
    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToUrl(string code)
    {
        var result = await _service.RedirectAsync(code, HttpContext);

        return Redirect(result);
    }
}