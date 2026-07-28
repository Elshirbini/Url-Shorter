using MassTransit;
using Microsoft.Extensions.Logging;
using UrlShorter.src.Common.Emails;
using UrlShorter.src.Common.Messaging.Contracts;
using UrlShorter.src.Modules.Emails.Enums;

namespace UrlShorter.src.Modules.Emails.Consumers;

public class SendEmailConsumer : IConsumer<SendEmailMessage>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmailConsumer> _logger;

    public SendEmailConsumer(IEmailService emailService, ILogger<SendEmailConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendEmailMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing email sending for template {Template} to {To}", message.Template, message.To);

        try
        {
            switch (message.Template)
            {
                case EmailTemplate.SendOtpConfirmation:
                    if (message.Data.TryGetValue("Otp", out var otpObj) && otpObj != null)
                    {
                        var otp = otpObj.ToString();
                        await _emailService.SendOtpAsync(message.To, otp, context.CancellationToken);
                    }
                    else
                    {
                        _logger.LogError("Missing or invalid 'Otp' in message data for SendOtpConfirmation");
                        throw new ArgumentException("Missing or invalid 'Otp'");
                    }
                    break;

                case EmailTemplate.SendResetPassword:
                    if (message.Data.TryGetValue("Code", out var codeObj) && codeObj != null)
                    {
                        var code = codeObj.ToString();
                        await _emailService.SendResetPasswordAsync(message.To, code, context.CancellationToken);
                    }
                    else
                    {
                        _logger.LogError("Missing or invalid 'Code' in message data for SendResetPassword");
                        throw new ArgumentException("Missing or invalid 'Code'");
                    }
                    break;

                default:
                    _logger.LogWarning("Unknown email template: {Template}", message.Template);
                    throw new NotSupportedException($"Email template {message.Template} is not supported");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw;
        }
    }
}
