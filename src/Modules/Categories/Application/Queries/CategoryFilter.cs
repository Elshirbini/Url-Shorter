namespace UrlShorter.src.Modules.Categories.Application.Queries;

public class CategoryFilter
{
    public Guid UserId { get; set; }

    public string? Search { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}