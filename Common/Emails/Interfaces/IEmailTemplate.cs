namespace UrlShorter.Common.Emails.Interfaces;

public interface IEmailTemplate
{
    string To { get; }
    string Subject();
    string Html();
}