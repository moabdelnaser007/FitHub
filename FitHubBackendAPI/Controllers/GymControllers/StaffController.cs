using FitHubBackendAPI.Services.Implementation.GymServices;
using FitHubBackendAPI.Services.Interfaces.GymBranch;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [Route("api/owner/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class StaffController : ControllerBase
    {
        
    }
}
