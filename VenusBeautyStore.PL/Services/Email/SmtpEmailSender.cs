//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.Extensions.Options;
//using MimeKit;

//public class SmtpOptions
//{
//    public string Host { get; set; } = "";
//    public int Port { get; set; } = 587;
//    public string User { get; set; } = "";
//    public string Pass { get; set; } = "";
//    public string From { get; set; } = "soporte@venusbeautystore.com";
//    public string FromName { get; set; } = "Venus Beauty";
//    public bool UseStartTls { get; set; } = true;
//}

//public class SmtpEmailSender : IEmailSender
//{
//    private readonly SmtpOptions _opt;
//    public SmtpEmailSender(IOptions<SmtpOptions> opt) => _opt = opt.Value;

//    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
//    {
//        var message = new MimeMessage();
//        message.From.Add(new MailboxAddress(_opt.FromName, _opt.From));
//        message.To.Add(MailboxAddress.Parse(email));
//        message.Subject = subject;

//        var body = new BodyBuilder { HtmlBody = htmlMessage };
//        message.Body = body.ToMessageBody();

//        using var client = new SmtpClient();
//        await client.ConnectAsync(_opt.Host, _opt.Port,
//            _opt.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
//        if (!string.IsNullOrWhiteSpace(_opt.User))
//            await client.AuthenticateAsync(_opt.User, _opt.Pass);

//        await client.SendAsync(message);
//        await client.DisconnectAsync(true);
//    }
//}
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

public class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string User { get; set; } = "";     // soporte@venusbeautystore.com
    public string Pass { get; set; } = "";     // contraseña (o App Password si hay MFA)
    public string FromName { get; set; } = "Venus Beauty";
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;
    public SmtpEmailSender(IOptions<SmtpOptions> opt) => _opt = opt.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // From = siempre el buzón autenticado
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_opt.FromName ?? _opt.User, _opt.User));
        msg.To.Add(MailboxAddress.Parse(email));
        msg.Subject = subject;
        msg.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

        // Si quieres traza, usa ProtocolLogger("smtp.log")
        using var client = new SmtpClient();

        await client.ConnectAsync(_opt.Host, _opt.Port, SecureSocketOptions.StartTls);
        client.AuthenticationMechanisms.Remove("XOAUTH2"); // fuerza user/pass

        await client.AuthenticateAsync(_opt.User, _opt.Pass);

        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }
}
