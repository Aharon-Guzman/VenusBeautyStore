using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

public class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string User { get; set; } = "";
    public string Pass { get; set; } = "";
    public string From { get; set; } = "";
    public string FromName { get; set; } = "Venus Beauty";
    public bool UseStartTls { get; set; } = true;
    public string SignatureImagePath { get; set; } = ""; 
    public string SignatureContentId { get; set; } = "vbs-signature";
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;
    private readonly IWebHostEnvironment _env;

    public SmtpEmailSender(IOptions<SmtpOptions> opt, IWebHostEnvironment env)
    {
        _opt = opt.Value;
        _env = env;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var msg = new MimeMessage();

        var fromAddress = string.IsNullOrWhiteSpace(_opt.From) ? _opt.User : _opt.From;
        msg.From.Add(new MailboxAddress(_opt.FromName ?? _opt.User, fromAddress));
        msg.To.Add(MailboxAddress.Parse(email));
        msg.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlMessage };

        // Firma inline (CID)
        var sigPath = ResolveSignaturePath(_opt.SignatureImagePath);
        if (!string.IsNullOrWhiteSpace(sigPath) && File.Exists(sigPath))
        {
            var lr = builder.LinkedResources.Add(sigPath);
            lr.ContentId = _opt.SignatureContentId;               
            lr.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
        }

        msg.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _opt.Host,
            _opt.Port,
            _opt.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto
        );
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        await client.AuthenticateAsync(_opt.User, _opt.Pass);
        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }

    private string ResolveSignaturePath(string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath)) return string.Empty;

        if (Path.IsPathRooted(configuredPath))
            return configuredPath;

        var basePath = !string.IsNullOrEmpty(_env.WebRootPath)
            ? _env.WebRootPath
            : _env.ContentRootPath;

        var relative = configuredPath
            .Replace("/", Path.DirectorySeparatorChar.ToString())
            .TrimStart(Path.DirectorySeparatorChar);

        return Path.Combine(basePath, relative);
    }
}
