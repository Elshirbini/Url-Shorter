using FluentValidation;
using UrlShorter.src.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.src.Modules.Auth.Presentation.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Identifier is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
