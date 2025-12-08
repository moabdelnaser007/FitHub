using FitHubBackendAPI.Entities;

namespace FitHubBackendAPI.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);

    }
}
