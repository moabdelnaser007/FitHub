namespace FitHubBackendAPI.Services.Interfaces.AuthServices
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
