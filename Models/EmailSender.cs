using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort))
        {
            client.Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);
            client.EnableSsl = _emailSettings.EnableSSL;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}

public class EmailSettings
{
    public string MailServer { get; set; }
    public int MailPort { get; set; }
    public string Sender { get; set; }
    public string SenderName { get; set; }
    public string Password { get; set; }
    public bool EnableSSL { get; set; }
}