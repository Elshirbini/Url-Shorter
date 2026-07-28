using FluentValidation;
using UrlShorter.src.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.src.Modules.Auth.Presentation.Validators;

public class ForgetPasswordDtoValidator : AbstractValidator<ForgetPasswordDto>
{
    public ForgetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");
    }
}
