namespace UrlShorter.src.Common.Messaging.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; init; } = default!;
    public ushort Port { get; init; }

    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;

    public string VirtualHost { get; init; } = "/";
}