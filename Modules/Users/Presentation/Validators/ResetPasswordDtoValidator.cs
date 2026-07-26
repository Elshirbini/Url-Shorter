using FluentValidation;
using UrlShorter.Modules.Users.Presentation.DTOs;

namespace UrlShorter.Modules.Users.Presentation.Validators;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Old Password is required")
            .MinimumLength(6).WithMessage("Old Password must be at least 6 characters")
            .MaximumLength(16).WithMessage("Old Password must not exceed 16 characters");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New Password is required")
            .MinimumLength(6).WithMessage("New Password must be at least 6 characters")
            .MaximumLength(16).WithMessage("New Password must not exceed 16 characters");
    }
}
