namespace UrlShorter.Common.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string message)
        : base(message, 400, "BAD_REQUEST") { }
}