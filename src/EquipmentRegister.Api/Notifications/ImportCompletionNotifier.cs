using System.Net;
using System.Net.Mail;
using EquipmentRegister.Api.Import;
using Microsoft.Extensions.Options;

namespace EquipmentRegister.Api.Notifications;

public interface IImportCompletionNotifier
{
    Task NotifyAsync(ImportResult result, CancellationToken cancellationToken = default);
}

public sealed class ImportEmailOptions
{
    public string SmtpHost { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 587;

    public bool EnableSsl { get; set; } = true;

    public string Sender { get; set; } = string.Empty;

    public string Recipient { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public string? Password { get; set; }
}

public sealed class SmtpImportCompletionNotifier(
    IOptions<ImportEmailOptions> options,
    ILogger<SmtpImportCompletionNotifier> logger) : IImportCompletionNotifier
{
    public async Task NotifyAsync(ImportResult result, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.SmtpHost)
            || string.IsNullOrWhiteSpace(settings.Sender)
            || string.IsNullOrWhiteSpace(settings.Recipient))
        {
            logger.LogWarning("Import completion email was not sent because SMTP settings are incomplete.");
            return;
        }

        using var message = new MailMessage(settings.Sender, settings.Recipient)
        {
            Subject = "Equipment import complete",
            Body = $"An equipment import has completed.{Environment.NewLine}{Environment.NewLine}" +
                $"Total rows: {result.TotalRows}{Environment.NewLine}" +
                $"Accepted: {result.Accepted}{Environment.NewLine}" +
                $"Rejected: {result.Rejected}",
            IsBodyHtml = false,
        };
        using var client = new SmtpClient(settings.SmtpHost, settings.SmtpPort)
        {
            EnableSsl = settings.EnableSsl,
            UseDefaultCredentials = false,
        };

        if (!string.IsNullOrWhiteSpace(settings.UserName))
        {
            client.Credentials = new NetworkCredential(settings.UserName, settings.Password);
        }

        await client.SendMailAsync(message, cancellationToken);
    }
}
