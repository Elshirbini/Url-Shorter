using UrlShorter.src.Modules.Emails.Enums;

namespace UrlShorter.src.Common.Messaging.Contracts;

public record SendEmailMessage(
    string To,
    EmailTemplate Template,
    IReadOnlyDictionary<string, object?> Data);