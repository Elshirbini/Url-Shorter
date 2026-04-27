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
<body style=""font-family:Arial; background:#f6f6f6; padding:20px;"">
    <div style=""background:#fff; padding:20px; border-radius:8px;"">
        <h2>مرحبًا 👋</h2>
        <p>رمز التحقق الخاص بك:</p>

        <div style=""text-align:center; margin:20px;"">
            <span style=""font-size:24px; letter-spacing:5px;"">
                {_otp}
            </span>
        </div>

        <p>صالح لمدة 10 دقائق فقط</p>
    </div>
</body>
</html>";
    }
}