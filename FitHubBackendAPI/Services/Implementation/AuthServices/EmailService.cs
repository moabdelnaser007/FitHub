using FitHubBackendAPI.Services.Interfaces.AuthServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FitHubBackendAPI.Services.Implementation.AuthServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body, bool isHtml = false)
        {
            var fromEmail = _config["EmailSettings:Email"];
            var appPassword = _config["EmailSettings:AppPassword"];
            var host = _config["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
            var useStartTls = bool.Parse(_config["EmailSettings:UseStartTls"] ?? "true");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FitHub", fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = isHtml
                ? new TextPart("html") { Text = body }
                : new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                // connect
                if (useStartTls)
                {
                    await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable);
                }
                else
                {
                    await smtp.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
                }

                // authenticate
                await smtp.AuthenticateAsync(fromEmail, appPassword);

                // send
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (SmtpCommandException ex)
            {
                // give clearer message to GlobalExceptionMiddleware
                throw new InvalidOperationException($"SMTP command error: {ex.Message}");
            }
            catch (SmtpProtocolException ex)
            {
                throw new InvalidOperationException($"SMTP protocol error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to send email: {ex.Message}");
            }
        }

        // Optionally, you can keep the old method for backward compatibility and delegate to the new one:
        public async Task SendAsync(string to, string subject, string body)
        {
            await SendAsync(to, subject, body, false);
        }
    }
}
