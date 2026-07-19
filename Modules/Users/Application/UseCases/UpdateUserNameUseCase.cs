using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Users.Application.Interfaces;
using UrlShorter.Modules.Users.Presentation.DTOs;

namespace UrlShorter.Modules.Users.Application.UseCases;

public class UpdateUserNameUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserNameUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> UpdateUserNameAsync(Guid userId, UpdateUserNameDto dto)
    {
        var exists = await _userRepository.UserExistsAsync(u => u.UserName == dto.UserName && u.UserId != userId);

        if (exists)
            throw new ConflictException("Username already taken");

        var user = await _userRepository.GetUserByIdAsync(userId)
            ?? throw new NotFoundException("User not found");

        user.UserName = dto.UserName;

        await _userRepository.SaveUserChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Username updated successfully"
        };
    }
}