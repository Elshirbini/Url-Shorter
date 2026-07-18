namespace UrlShorter.Modules.Links.Application.Queries;

public class LinkFilter
{
    public Guid UserId { get; set; }
    public Guid? LinkId { get; set; }

    public Guid? CategoryId { get; set; }

    public string? Search { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}