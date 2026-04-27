using UrlShorter.Common.Emails.Interfaces;

namespace UrlShorter.Common.Emails.Templates;

public class ResetPasswordTemplate : IEmailTemplate
{
    private readonly string _recipient;
    private readonly string _code;

    public ResetPasswordTemplate(string recipient, string code)
    {
        _recipient = recipient;
        _code = code;
    }

    public string To => _recipient;

    public string Subject()
    {
        return "🔑 إعادة تعيين كلمة المرور – معاك";
    }

    public string Html()
    {
        return $@"
<!DOCTYPE html>
<html lang=""ar"" dir=""rtl"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f4f4;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f4f4f4;"">
    <tr>
      <td align=""center"" style=""padding: 20px;"">
        <div style=""font-family: Arial, sans-serif; line-height: 1.6; max-width: 600px; width: 100%; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; direction: rtl; text-align: right;"">
          
          <div style=""background-color: #ffffff; padding: 30px; border-radius: 10px;"">
            <h2 style=""color: #333;"">🔐 إعادة تعيين كلمة المرور</h2>

            <p style=""font-size: 16px; color: #555;"">
              لقد طلبت إعادة تعيين كلمة المرور لحسابك على <strong>معاك</strong>. استخدم الرمز التالي لإكمال العملية:
            </p>

            <div style=""text-align: center; margin: 20px 0;"">
              <span style=""font-size: 28px; font-weight: bold; letter-spacing: 4px; color: #007bff;"">
                {_code}
              </span>
            </div>

            <p style=""font-size: 14px; color: #999;"">
              ⚠️ هذا الرمز صالح لمدة <strong>10 دقائق فقط</strong>.
              إذا لم تطلب إعادة التعيين، يمكنك تجاهل هذا البريد بأمان.
            </p>

            <p style=""font-size: 14px; color: #555; margin-top: 20px;"">
              مع تحيات فريق <strong>كرد</strong> 💙
            </p>
          </div>

        </div>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}