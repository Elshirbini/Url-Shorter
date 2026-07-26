using FluentValidation;
using UrlShorter.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.Modules.Auth.Presentation.Validators;

public class VerifyCodeDtoValidator : AbstractValidator<VerifyCodeDto>
{
    public VerifyCodeDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required");
    }
}
