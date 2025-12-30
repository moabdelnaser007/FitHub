using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Interfaces.AdminServices
{
    public interface IAdminRevenue
    {
        Task<ResponseViewModel<decimal>> GetAdminAllRevenue(
                    DateTime? startFrom,
                    DateTime? endWith);
    }
}
