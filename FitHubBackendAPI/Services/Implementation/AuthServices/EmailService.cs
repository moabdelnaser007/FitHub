using FitHubBackendAPI.Services.Interfaces.AuthServices;

namespace FitHubBackendAPI.Services.Implementation.AuthServices
{
    public class EmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            Console.WriteLine("===== EMAIL SENT =====");
            Console.WriteLine($"To: {to}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
            Console.WriteLine("======================");

            return Task.CompletedTask;
        }
    }
}
