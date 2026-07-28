namespace UrlShorter.src.Common.Storage.Models;

public sealed class UploadFileResult
{
    public required string Key { get; init; }

    public required string Url { get; init; }

}