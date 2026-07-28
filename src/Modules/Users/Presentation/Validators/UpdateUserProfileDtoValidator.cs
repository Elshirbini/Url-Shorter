using FluentValidation;
using UrlShorter.src.Modules.Users.Presentation.DTOs;

namespace UrlShorter.src.Modules.Users.Presentation.Validators;

public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required")
            .MinimumLength(3).WithMessage("UserName must be at least 3 characters long")
            .MaximumLength(50).WithMessage("UserName must be at most 50 characters long");
    }
}
