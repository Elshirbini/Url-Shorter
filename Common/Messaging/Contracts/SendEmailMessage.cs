using UrlShorter.Modules.Emails.Enums;

namespace UrlShorter.Common.Messaging.Contracts;

public record SendEmailMessage(
    string To,
    EmailTemplate Template,
    IReadOnlyDictionary<string, object?> Data);