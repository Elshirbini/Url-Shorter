namespace UrlShorter.Common.Emails;

public interface IEmailService
{
    Task SendOtpAsync(string to, string otp, CancellationToken cancellationToken = default);
    Task SendResetPasswordAsync(string to, string code, CancellationToken cancellationToken = default);
}