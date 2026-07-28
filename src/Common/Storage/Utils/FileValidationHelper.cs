using MimeDetective;
using UrlShorter.src.Common.Exceptions;

namespace UrlShorter.src.Common.Storage.Utils;

public static class FileValidationHelper
{
    private static readonly IContentInspector Inspector =
        new ContentInspectorBuilder
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        }.Build();

    public static async Task ValidateFileAsync(
        Stream stream,
        long size,
        string contentType,
        IReadOnlySet<string> allowedContentTypes,
        long maxSizeInBytes,
        CancellationToken cancellationToken = default)
    {
        ValidateSize(size, maxSizeInBytes);

        ValidateContentType(contentType, allowedContentTypes);

        ValidateMagicBytesAsync(
           stream,
           contentType,
           cancellationToken);
    }

    public static void ValidateSize(
        long size,
        long maxSizeInBytes)
    {
        if (size <= 0)
            throw new BadRequestException("File is empty.");

        if (size > maxSizeInBytes)
            throw new BadRequestException(
                $"Maximum allowed file size is {maxSizeInBytes} bytes.");
    }

    public static void ValidateContentType(
        string contentType,
        IReadOnlySet<string> allowedContentTypes)
    {
        if (!allowedContentTypes.Contains(contentType))
            throw new BadRequestException(
                $"Content type '{contentType}' is not allowed.");
    }

    public static void ValidateMagicBytesAsync(
        Stream stream,
        string expectedContentType,
        CancellationToken cancellationToken = default)
    {
        if (!stream.CanSeek)
        {
            throw new BadRequestException("The provided stream does not support seeking.");
        }

        stream.Position = 0;

        var result = Inspector.Inspect(stream);
        stream.Position = 0;
        var detectedMimeType = result
            .ByMimeType()
            .FirstOrDefault()?
            .MimeType;

        if (true)
        {
            Console.WriteLine("1");
        }

        if (!string.Equals(
                detectedMimeType,
                expectedContentType,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException(
                  "The uploaded file content does not match its content type.");
        }

    }
}