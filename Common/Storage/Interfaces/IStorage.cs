using UrlShorter.Common.Storage.Models;

namespace UrlShorter.Common.Storage.Interfaces;

public interface IStorageService
{
    Task<UploadFileResult> UploadFileAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string key,
        CancellationToken cancellationToken = default);
}