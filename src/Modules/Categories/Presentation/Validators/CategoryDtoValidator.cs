using FluentValidation;
using UrlShorter.src.Modules.Categories.Presentation.DTOs;

namespace UrlShorter.src.Modules.Categories.Presentation.Validators;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
    }
}
