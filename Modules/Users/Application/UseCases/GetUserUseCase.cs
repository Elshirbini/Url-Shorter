using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Modules.Users.Application.Interfaces;

namespace UrlShorter.Modules.Users.Application.UseCases;

public class GetUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> GetUserAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new NotFoundException("User not found");
        return new ApiResponse<object>
        {
            Success = true,
            Data = user
        };
    }
}