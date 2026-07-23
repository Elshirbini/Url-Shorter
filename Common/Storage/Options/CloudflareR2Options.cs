namespace UrlShorter.Common.Storage.Options;

public class CloudflareR2Options
{
    public string Endpoint { get; set; } = string.Empty;
    public string PublicDomain { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
}