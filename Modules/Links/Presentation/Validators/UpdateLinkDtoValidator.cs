using FluentValidation;
using System;
using UrlShorter.Modules.Links.Presentation.DTOs;

namespace UrlShorter.Modules.Links.Presentation.Validators;

public class UpdateLinkDtoValidator : AbstractValidator<UpdateLinkDto>
{
    public UpdateLinkDtoValidator()
    {
        RuleFor(x => x.Code)
            .Length(6).WithMessage("Code must be 6 characters")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.RedirectUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Invalid URL")
            .When(x => !string.IsNullOrEmpty(x.RedirectUrl));
    }
}
