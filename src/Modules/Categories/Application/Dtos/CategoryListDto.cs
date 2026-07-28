namespace UrlShorter.src.Modules.Categories.Application.Dtos;

public class CategoryListDto
{
    public Guid CategoryId { get; set; }
    public string? Name { get; set; }
    public int? LinksCount { get; set; }
}