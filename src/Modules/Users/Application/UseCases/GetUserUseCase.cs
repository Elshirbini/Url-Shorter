using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Modules.Users.Application.Interfaces;

namespace UrlShorter.src.Modules.Users.Application.UseCases;

public class GetUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found");
        return new ApiResponse<object>
        {
            Success = true,
            Data = user
        };
    }
}