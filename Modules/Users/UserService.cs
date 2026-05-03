using Microsoft.EntityFrameworkCore;
using UrlShorter.Common.Exceptions;
using UrlShorter.Common.Responses;
using UrlShorter.Data;
using UrlShorter.Modules.Users.DTOs;

namespace UrlShorter.Modules.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<object>> GetUserAsync(Guid userId)
    {
        var user = await _db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new
            {
                u.UserId,
                u.UserName,
                u.Email,
                u.CreatedAt
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found");
        return new ApiResponse<object>
        {
            Success = true,
            Data = user
        };
    }

    public async Task<ApiResponse<object>> UpdateUserNameAsync(Guid userId, UpdateUserNameDto dto)
    {
        var exists = await _db.Users
            .AnyAsync(u => u.UserName == dto.UserName && u.UserId != userId);

        if (exists)
            throw new ConflictException("Username already taken");

        var user = await _db.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found");

        user.UserName = dto.UserName;

        await _db.SaveChangesAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Username updated successfully"
        };
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(HttpContext context, Guid userId, ResetPasswordDto dto)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
            throw new UnauthorizedException("Old password is incorrect");

        var hashed = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        user.Password = hashed;

        await _db.SaveChangesAsync();

        context.Response.Cookies.Delete("accessToken");
        context.Response.Cookies.Delete("refreshToken");

        return new ApiResponse<object>
        {
            Success = true,
            Message = "Password updated successfully"
        };
    }
}