using System.Security.Claims;
using UrlShorter.Modules.Users.Models;

namespace UrlShorter.Common.Security;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string token, string jti) GenerateRefreshToken(User user);
    ClaimsPrincipal? ValidateRefreshToken(string token);
}