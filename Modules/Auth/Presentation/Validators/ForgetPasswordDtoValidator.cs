using FluentValidation;
using UrlShorter.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.Modules.Auth.Presentation.Validators;

public class ForgetPasswordDtoValidator : AbstractValidator<ForgetPasswordDto>
{
    public ForgetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");
    }
}
