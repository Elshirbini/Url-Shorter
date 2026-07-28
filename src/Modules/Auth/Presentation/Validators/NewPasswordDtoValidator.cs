using FluentValidation;
using UrlShorter.src.Modules.Auth.Presentation.DTOs;

namespace UrlShorter.src.Modules.Auth.Presentation.Validators;

public class NewPasswordDtoValidator : AbstractValidator<NewPasswordDto>
{
    public NewPasswordDtoValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty().WithMessage("Reset token is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(16).WithMessage("Password must not exceed 16 characters");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required")
            .MinimumLength(6).WithMessage("Confirm Password must be at least 6 characters")
            .MaximumLength(16).WithMessage("Confirm Password must not exceed 16 characters");
    }
}
