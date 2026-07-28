using System.Security.Claims;
using UrlShorter.src.Modules.Users.Infrastructure.Models;

namespace UrlShorter.src.Common.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string token, string jti) GenerateRefreshToken(User user);
    ClaimsPrincipal? ValidateRefreshToken(string token);
}