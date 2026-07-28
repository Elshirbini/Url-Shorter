namespace UrlShorter.src.Common.Storage.Models;

public sealed class UploadFileRequest
{
    public required string Key { get; init; }

    public required Stream FileContent { get; init; }

    public required string ContentType { get; init; }
}