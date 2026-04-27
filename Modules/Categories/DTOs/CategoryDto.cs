using System.ComponentModel.DataAnnotations;

namespace UrlShorter.Modules.Categories.DTOs;

public class CategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
}