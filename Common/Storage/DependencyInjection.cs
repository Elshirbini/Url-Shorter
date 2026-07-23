using UrlShorter.Common.Storage.Interfaces;
using UrlShorter.Common.Storage.Options;
using UrlShorter.Common.Storage.Providers;

namespace UrlShorter.Common.Storage;

public static class DependencyInjection
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<CloudflareR2Options>()
            .BindConfiguration("CloudflareR2");

        services.AddScoped<IStorageService, CloudflareR2Service>();

        return services;
    }
}