using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymOwnerController : ControllerBase
    {

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public ResponseViewModel<CreateGymBranchDTO> CreateGymBranch(CreateGymBranchDTO dto)
        {
            // Implementation for creating a gym branch would go here.
            // For now, we return a success response with the provided DTO.
            return ResponseViewModel<CreateGymBranchDTO>.Success(dto, "Gym branch created successfully.");
        }
    }
}
