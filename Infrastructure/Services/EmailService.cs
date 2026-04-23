using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Application.DTOs.AuthDTOs;

namespace Infrastructure.Services;

public class EmailService(IOptions<SmtpSettings> options, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendAsync(EmailMessageDto message)
    {
        try
        {
            var smtp = options.Value;
            using var client = new SmtpClient(smtp.Host, smtp.Port);
            client.Credentials = new NetworkCredential(smtp.Username, smtp.Password);
            client.EnableSsl = true;

            var mail = new MailMessage
            {
                From = new MailAddress(smtp.From, smtp.DisplayName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml
            };
            mail.To.Add(message.To);

            await client.SendMailAsync(mail);
            logger.LogInformation("Email sent to {Recipient}", message.To);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Recipient}", message.To);
            throw;
        }
    }
}
