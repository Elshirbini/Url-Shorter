namespace UrlShorter.Common.Storage.Utils;

public static class FileExtensionHelper
{
    public static string GetExtension(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            // Images
            "image/jpeg" => ".jpg",
            "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/tiff" => ".tiff",
            "image/svg+xml" => ".svg",
            "image/x-icon" => ".ico",
            "image/heic" => ".heic",
            "image/heif" => ".heif",
            "image/avif" => ".avif",

            // Documents
            "application/pdf" => ".pdf",
            "text/plain" => ".txt",
            "text/csv" => ".csv",

            // Microsoft Office
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",

            "application/vnd.ms-excel" => ".xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",

            "application/vnd.ms-powerpoint" => ".ppt",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => ".pptx",

            // Archives
            "application/zip" => ".zip",
            "application/x-rar-compressed" => ".rar",
            "application/x-7z-compressed" => ".7z",
            "application/gzip" => ".gz",
            "application/x-tar" => ".tar",

            // Audio
            "audio/mpeg" => ".mp3",
            "audio/wav" => ".wav",
            "audio/ogg" => ".ogg",
            "audio/aac" => ".aac",

            // Video
            "video/mp4" => ".mp4",
            "video/webm" => ".webm",
            "video/x-msvideo" => ".avi",
            "video/quicktime" => ".mov",
            "video/x-matroska" => ".mkv",

            _ => throw new NotSupportedException(
                $"Unsupported content type '{contentType}'.")
        };
    }
}