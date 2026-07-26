using FluentValidation;
using System;
using UrlShorter.Modules.Links.Presentation.DTOs;

namespace UrlShorter.Modules.Links.Presentation.Validators;

public class CreateLinkDtoValidator : AbstractValidator<CreateLinkDto>
{
    public CreateLinkDtoValidator()
    {
        RuleFor(x => x.Code)
            .Length(6).WithMessage("Code must be 6 characters")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.RedirectUrl)
            .NotEmpty().WithMessage("RedirectUrl is required") // Or just "The RedirectUrl field is required" based on [Required]
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Invalid URL");
    }
}
