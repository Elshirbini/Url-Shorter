using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Auth.Presentation.DTOs;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Auth.Application.UseCases
{
    public class VerifyCodeUseCase
    {
        private readonly IUserRepository _userRepository;

        public VerifyCodeUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<object>> VerifyCodeAsync(VerifyCodeDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetFirstOrDefaultUserAsync(u => u.Email == dto.Email, cancellationToken);

            if (user == null || user.CodeValidation == null)
                throw new BadRequestException("Invalid code");

            if (user.CodeValidationExpire < DateTime.UtcNow)
                throw new BadRequestException("Code expired");

            var isValid = BCrypt.Net.BCrypt.Verify(dto.Code, user.CodeValidation);

            if (!isValid)
                throw new BadRequestException("Invalid code");

            var resetToken = Guid.NewGuid().ToString();

            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpire = DateTime.UtcNow.AddMinutes(15);

            user.CodeValidation = null;
            user.CodeValidationExpire = null;

            await _userRepository.SaveUserChangesAsync(cancellationToken);

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Code verified",
                Data = new { resetToken }
            };
        }
    }
}