namespace UrlShorter.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND") { }
}