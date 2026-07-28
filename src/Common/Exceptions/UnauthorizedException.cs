namespace UrlShorter.src.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base(message, 401, "UNAUTHORIZED") { }
}