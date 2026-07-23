using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Common.Storage.Constants;
using UrlShorter.Common.Storage.Interfaces;
using UrlShorter.Common.Storage.Models;
using UrlShorter.Common.Storage.Utils;
using UrlShorter.Modules.Users.Application.Interfaces;
using UrlShorter.Modules.Users.Presentation.DTOs;

namespace UrlShorter.Modules.Users.Application.UseCases;

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

    public async Task<ApiResponse<object>> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto)
    {
        string? oldImageKey = null;

        var exists = await _userRepository.UserExistsAsync(u => u.UserName == dto.UserName && u.UserId != userId);

        if (exists)
            throw new ConflictException("Username already taken");


        var user = await _userRepository.GetUserByIdAsync(userId)
            ?? throw new NotFoundException("User not found");

        if (dto.ProfileImage != null)
        {
            _logger.LogInformation("Uploading profile image for user:{userId} content-type:{contentType}, size:{size}", userId, dto.ProfileImage.ContentType, dto.ProfileImage.Length);

            await using var stream = dto.ProfileImage.OpenReadStream();

            await FileValidationHelper.ValidateFileAsync(stream, dto.ProfileImage.Length, dto.ProfileImage.ContentType, FileTypes.Images, 5 * 1024 * 1024);

            oldImageKey = user.ImageKey;

            var result = await _storage.UploadFileAsync(
                new UploadFileRequest
                {
                    Key = "users/" + userId.ToString(),
                    FileContent = stream,
                    ContentType = dto.ProfileImage.ContentType
                });

            user.ImageKey = result.Key;
            user.ImageUrl = result.Url;

        }

        user.UserName = dto.UserName ?? user.UserName;

        await _userRepository.SaveUserChangesAsync();

        if (!string.IsNullOrWhiteSpace(oldImageKey))
        {
            try
            {
                await _storage.DeleteFileAsync(oldImageKey);
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