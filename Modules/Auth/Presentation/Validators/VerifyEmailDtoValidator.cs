using FluentValidation;
using UrlShorter.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.Modules.Auth.Presentation.Validators;

public class VerifyEmailDtoValidator : AbstractValidator<VerifyEmailDto>
{
    public VerifyEmailDtoValidator()
    {
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required")
            .MaximumLength(6).WithMessage("OTP must be at most 6 characters");
    }
}
