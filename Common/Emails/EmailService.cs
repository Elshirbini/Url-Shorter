using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UrlShorter.Common.Emails.Interfaces;
using UrlShorter.Common.Emails.Templates;

namespace UrlShorter.Common.Emails;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(HttpClient httpClient, IConfiguration config, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    private async Task SendEmail(string to, string subject, string html)
    {
        var apiKey = _config["Resend:ApiKey"];
        var from = _config["Resend:FromEmail"];

        var body = new
        {
            from,
            to = new[] { to },
            subject,
            html
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        _logger.LogInformation("Sending email to {Email}", to);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();

            _logger.LogError("Email sending failed");
            throw new Exception($"Email failed: {errorBody}");
        }

        _logger.LogInformation("Email sent successfully");
    }

    private Task SendTemplate(IEmailTemplate template)
    {
        return SendEmail(template.To, template.Subject(), template.Html());
    }

    public async Task SendOtpAsync(string to, string otp)
    {
        var template = new OtpConfirmationTemplate(to, otp);
        await SendTemplate(template);
    }

    public async Task SendResetPasswordAsync(string to, string code)
    {
        var template = new ResetPasswordTemplate(to, code);
        await SendTemplate(template);
    }
}
