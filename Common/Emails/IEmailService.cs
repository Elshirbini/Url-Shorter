namespace UrlShorter.Common.Emails;

public interface IEmailService
{
    Task SendOtpAsync(string to, string otp);
    Task SendResetPasswordAsync(string to, string code);
}