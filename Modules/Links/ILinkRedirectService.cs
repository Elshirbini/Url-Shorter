namespace UrlShorter.Modules.Links;

public interface ILinkRedirectService
{
    Task<string> RedirectAsync(string code, HttpContext context);
}