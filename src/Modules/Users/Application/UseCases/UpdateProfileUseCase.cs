using UrlShorter.src.Common.Exceptions;
using UrlShorter.src.Common.Responses;
using UrlShorter.src.Common.Storage.Constants;
using UrlShorter.src.Common.Storage.Interfaces;
using UrlShorter.src.Common.Storage.Models;
using UrlShorter.src.Common.Storage.Utils;
using UrlShorter.src.Modules.Users.Application.Interfaces;
using UrlShorter.src.Modules.Users.Presentation.DTOs;

namespace UrlShorter.src.Modules.Users.Application.UseCases;

public class UpdateUserProfileUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IStorageService _storage;
    private readonly ILogger<UpdateUserProfileUseCase> _logger;

    public UpdateUserProfileUseCase(IUserRepository userRepository, IStorageService storage, ILogger<UpdateUserProfileUseCase> logger)
    {
        _userRepository = userRepository;
        _storage = storage;
        _logger = logger;
    }

    public async Task<ApiResponse<object>> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto, CancellationToken cancellationToken)
    {
        string? oldImageKey = null;

        var exists = await _userRepository.UserExistsAsync(u => u.UserName == dto.UserName && u.UserId != userId, cancellationToken);

        if (exists)
            throw new ConflictException("Username already taken");


        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found");

        if (dto.ProfileImage != null)
        {
            _logger.LogInformation("Uploading profile image for user:{userId} content-type:{contentType}, size:{size}", userId, dto.ProfileImage.ContentType, dto.ProfileImage.Length);

            await using var stream = dto.ProfileImage.OpenReadStream();

            await FileValidationHelper.ValidateFileAsync(stream, dto.ProfileImage.Length, dto.ProfileImage.ContentType, FileTypes.Images, 5 * 1024 * 1024, cancellationToken);

            oldImageKey = user.ImageKey;

            var result = await _storage.UploadFileAsync(
                new UploadFileRequest
                {
                    Key = "users/" + userId.ToString(),
                    FileContent = stream,
                    ContentType = dto.ProfileImage.ContentType
                }, cancellationToken);

            user.ImageKey = result.Key;
            user.ImageUrl = result.Url;

        }

        user.UserName = dto.UserName ?? user.UserName;

        await _userRepository.SaveUserChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldImageKey))
        {
            try
            {
                await _storage.DeleteFileAsync(oldImageKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to delete old profile image {ImageKey}",
                    oldImageKey);
            }
        }

        return new ApiResponse<object>
        {
            Success = true,
            Message = "User updated successfully"
        };
    }
}