using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Uný_Proje.Services
{
    public class EmailServis
    {
        private readonly IConfiguration _configuration;

        public EmailServis(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> EmailGonder(string aliciEmail, string konu, string htmlIcerik)
        {
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("EMAIL GONDERME ISLEMI BASLADI");
                Console.WriteLine("==================================================");
                Console.WriteLine($"Alici: {aliciEmail}");
                Console.WriteLine($"Konu: {konu}");
                Console.WriteLine($"SMTP Server: {_configuration["EmailSettings:SmtpServer"]}");
                Console.WriteLine($"SMTP Port: {_configuration["EmailSettings:SmtpPort"]}");
                Console.WriteLine($"Sender Email: {_configuration["EmailSettings:SenderEmail"]}");
                Console.WriteLine($"Password Length: {_configuration["EmailSettings:SenderPassword"]?.Length ?? 0}");

                var email = new MimeMessage();
                
                // Gönderen
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"] ?? "Ikinci El Market";
                
                Console.WriteLine($"From: {senderName} <{senderEmail}>");
                email.From.Add(new MailboxAddress(senderName, senderEmail));

                // Alýcý
                Console.WriteLine($"To: {aliciEmail}");
                email.To.Add(MailboxAddress.Parse(aliciEmail));

                // Konu
                email.Subject = konu;

                // Ýçerik (HTML)
                var builder = new BodyBuilder
                {
                    HtmlBody = htmlIcerik
                };
                email.Body = builder.ToMessageBody();

                Console.WriteLine("SMTP baglantisi kuruluyor...");

                // SMTP ile gönder
                using var smtp = new SmtpClient();
                
                // SMTP loglama aktif et
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                
                Console.WriteLine($"Connecting to {smtpServer}:{smtpPort}...");
                
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);

                Console.WriteLine("SMTP kimlik dogrulamasi yapiliyor...");

                var password = _configuration["EmailSettings:SenderPassword"];
                Console.WriteLine($"Authenticating with email: {senderEmail}");
                
                await smtp.AuthenticateAsync(senderEmail, password);

                Console.WriteLine("Email gonderiliyor...");

                await smtp.SendAsync(email);
                
                Console.WriteLine("Baglanti kapatiliyor...");
                await smtp.DisconnectAsync(true);

                Console.WriteLine("==================================================");
                Console.WriteLine("EMAIL BASARIYLA GONDERILDI!");
                Console.WriteLine("==================================================");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("EMAIL GONDERME HATASI!");
                Console.WriteLine("==================================================");
                Console.WriteLine($"Hata Mesaji: {ex.Message}");
                Console.WriteLine($"Hata Tipi: {ex.GetType().Name}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().Name}");
                }
                
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("==================================================");
                
                return false;
            }
        }

        public string SifreResetEmailHtml(string kullaniciAdi, string resetKodu)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; color: #0d6efd; margin-bottom: 30px; }}
        .logo {{ font-size: 48px; margin-bottom: 10px; }}
        .code-box {{ background-color: #f8f9fa; border: 2px dashed #0d6efd; padding: 20px; text-align: center; font-size: 32px; font-weight: bold; color: #0d6efd; letter-spacing: 5px; margin: 20px 0; border-radius: 5px; }}
        .info {{ color: #666; line-height: 1.6; margin: 20px 0; }}
        .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; color: #856404; }}
        .warning-icon {{ color: #ffc107; font-weight: bold; font-size: 18px; }}
        .footer {{ text-align: center; color: #999; font-size: 12px; margin-top: 30px; border-top: 1px solid #eee; padding-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>&#128722;</div>
            <h1 style='color: #0d6efd; margin: 0;'>Ýkinci El Market</h1>
            <h2 style='color: #6c757d; font-weight: normal;'>Þifre Sýfýrlama</h2>
        </div>
        
        <p>Merhaba <strong>{kullaniciAdi}</strong>,</p>
        
        <p class='info'>Þifre sýfýrlama talebiniz alýndý. Aþaðýdaki doðrulama kodunu kullanarak yeni þifrenizi oluþturabilirsiniz:</p>
        
        <div class='code-box'>
            {resetKodu}
        </div>
        
        <p class='info'>Bu kod <strong>15 dakika</strong> süreyle geçerlidir.</p>
        
        <div class='warning'>
            <span class='warning-icon'>&#9888;</span> <strong>Önemli:</strong> Bu talebi siz oluþturmadýysanýz, bu e-postayý dikkate almayýn ve þifrenizi deðiþtirin.
        </div>
        
        <div class='footer'>
            <p>Bu bir otomatik e-postadýr, lütfen yanýtlamayýn.</p>
            <p>&copy; 2025 Ýkinci El Market. Tüm haklarý saklýdýr.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
