using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShorter.src.Common.Messaging.Options;
using UrlShorter.src.Modules.Emails.Consumers;

namespace UrlShorter.src.Common.Messaging.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        var options = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>()!;

        services.AddMassTransit(x =>
        {
            x.AddConsumer<SendEmailConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    options.Host,
                    options.Port,
                    options.VirtualHost,
                    h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}