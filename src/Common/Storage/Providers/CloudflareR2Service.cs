using UrlShorter.src.Common.Storage.Interfaces;
using UrlShorter.src.Common.Storage.Models;
using UrlShorter.src.Common.Storage.Options;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Microsoft.Extensions.Options;
using UrlShorter.src.Common.Storage.Utils;

namespace UrlShorter.src.Common.Storage.Providers;

public class CloudflareR2Service : IStorageService
{
    private readonly CloudflareR2Options _options;
    private readonly AmazonS3Client _client;
    private readonly ILogger<CloudflareR2Service> _logger;

    public CloudflareR2Service(
       IOptions<CloudflareR2Options> options,
       ILogger<CloudflareR2Service> logger)
    {
        _options = options.Value;
        _logger = logger;

        var config = new AmazonS3Config
        {
            ServiceURL = _options.Endpoint,
            ForcePathStyle = true
        };

        _client = new AmazonS3Client(
            _options.AccessKeyId,
            _options.SecretAccessKey,
            config);
    }

    public async Task<UploadFileResult> UploadFileAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken = default)
    {


        var key = request.Key + FileExtensionHelper.GetExtension(request.ContentType);

        try
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = request.FileContent,
                ContentType = request.ContentType,
                AutoCloseStream = false,
                DisablePayloadSigning = true,

            };

            await _client.PutObjectAsync(putObjectRequest, cancellationToken);

            return new UploadFileResult
            {
                Key = key,
                Url = $"{_options.PublicDomain.TrimEnd('/')}/{key}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload file to Cloudflare R2. Bucket: {Bucket}, Key: {Key}",
                _options.BucketName,
                key);

            throw;
        }
    }

    public async Task DeleteFileAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _client.DeleteObjectAsync(
                deleteObjectRequest,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to delete file from Cloudflare R2. Bucket: {Bucket}, Key: {Key}",
                _options.BucketName,
                key);

            throw;
        }
    }

}