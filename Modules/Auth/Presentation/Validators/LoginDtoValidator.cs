using FluentValidation;
using UrlShorter.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.Modules.Auth.Presentation.Validators;

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
