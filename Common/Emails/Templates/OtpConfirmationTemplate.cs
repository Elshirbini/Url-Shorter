using UrlShorter.Common.Emails.Interfaces;

namespace UrlShorter.Common.Emails.Templates;

public class OtpConfirmationTemplate : IEmailTemplate
{
    private readonly string _recipient;
    private readonly string _otp;

    public OtpConfirmationTemplate(string recipient, string otp)
    {
        _recipient = recipient;
        _otp = otp;
    }

    public string To => _recipient;

    public string Subject()
    {
        return "تأكيد حسابك";
    }

    public string Html()
    {
        return $@"
<!DOCTYPE html>
<html lang=""ar"" dir=""rtl"">
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>تأكيد حسابك</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f7f6;
            margin: 0;
            padding: 0;
            -webkit-font-smoothing: antialiased;
            -webkit-text-size-adjust: none;
        }}
        .email-wrapper {{
            width: 100%;
            background-color: #f4f7f6;
            padding: 20px 0;
        }}
        .email-content {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
            overflow: hidden;
        }}
        .header {{
            background-color: #4a90e2;
            color: #ffffff;
            padding: 20px;
            text-align: center;
        }}
        .header h2 {{
            margin: 0;
            font-size: 24px;
        }}
        .body {{
            padding: 30px;
            text-align: center;
            color: #333333;
        }}
        .otp-container {{
            background-color: #f8f9fa;
            border: 2px dashed #4a90e2;
            border-radius: 8px;
            padding: 20px;
            margin: 20px auto;
            max-width: 300px;
        }}
        .otp-code {{
            font-size: 32px;
            font-weight: bold;
            color: #4a90e2;
            letter-spacing: 8px;
            margin: 0;
        }}
        .footer {{
            padding: 20px;
            text-align: center;
            color: #888888;
            font-size: 14px;
            border-top: 1px solid #eeeeee;
        }}
        @media only screen and (max-width: 600px) {{
            .email-content {{
                width: 100% !important;
                border-radius: 0 !important;
            }}
            .body {{
                padding: 20px !important;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-wrapper"">
        <div class=""email-content"">
            <div class=""header"">
                <h2>مرحبًا بك 👋</h2>
            </div>
            <div class=""body"">
                <p style=""font-size: 16px; line-height: 1.5;"">لقد طلبت رمز تحقق للوصول إلى حسابك. يرجى استخدام الرمز التالي:</p>
                <div class=""otp-container"">
                    <p class=""otp-code"">{_otp}</p>
                </div>
                <p style=""font-size: 14px; color: #666; margin-top: 20px;"">هذا الرمز صالح لمدة <strong>10 دقائق</strong> فقط. لا تشارك هذا الرمز مع أي شخص.</p>
            </div>
            <div class=""footer"">
                <p style=""margin: 0;"">إذا لم تطلب هذا الرمز، يمكنك تجاهل هذه الرسالة بأمان.</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }
}