using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Services.Interfaces.AuthServices
{
    public interface IJwtService
    {
        string GenerateToken(User user);

    }
}
