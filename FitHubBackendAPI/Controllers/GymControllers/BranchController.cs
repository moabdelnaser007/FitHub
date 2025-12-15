using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Services.Interfaces.GymBranch;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitHubBackendAPI.Controllers.GymControllers
{
    [Route("api/owner/[controller]/[action]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        IGymBranchService _gymBranchService;
        public BranchController(IGymBranchService gymBranchService)
        {
            _gymBranchService = gymBranchService;
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public ResponseViewModel<CreateGymBranchDTO> CreateBranch(CreateGymBranchDTO dto)
        {

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string userName = User.FindFirst(ClaimTypes.Name)!.Value;
            _gymBranchService.CreateGymBranchAsync(userId, dto);

            return ResponseViewModel<CreateGymBranchDTO>.Success(dto, "Gym branch created successfully.");
        }
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<ResponseViewModel<GetGymBranchByIdDTO>> UpdateBranch(UpdateGymBranchDTO dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.UpdateGymBranchAsync(userId, dto);
            var branch = await _gymBranchService.GetGymBranchByIdAsync(userId);
            return ResponseViewModel<GetGymBranchByIdDTO>.Success(branch, "Gym branch updated successfully.");
        }
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public ResponseViewModel<AddStuffDTO> AddStuff(AddStuffDTO dto)
        {
            // Implementation for adding stuff to a gym branch would go here.
            // For now, we return a success response with the provided DTO.
            return ResponseViewModel<AddStuffDTO>.Success(dto, "Stuff added successfully.");
        }
        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<ResponseViewModel<IEnumerable<GetAllBranchDTO>>> GetAllBranchesAsync()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var branches = await _gymBranchService.GetAllBranchesAsync(userId);
            return ResponseViewModel<IEnumerable<GetAllBranchDTO>>.Success(branches, "Branches retrieved successfully.");
        }
        [HttpGet]
        [Authorize]
        [Route("{id:int}")]
        public async Task<ResponseViewModel<GetGymBranchByIdDTO>> GetBranchById(int id)
        {
            var branch = await _gymBranchService.GetGymBranchByIdAsync(id);
            return ResponseViewModel<GetGymBranchByIdDTO>.Success(branch, "Branche retrieved successfully.");

        }

        [HttpPut]
        [Authorize(Roles = "Owner")]
        [Route("{id:int}")]
        public async Task<ResponseViewModel<bool>> ActivateGymBranch(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.ActivateGymBranchAsync(userId, id);
            return ResponseViewModel<bool>.Success(true, "Branche Activated successfully.");

        }
        [HttpPut]
        [Authorize(Roles = "Owner")]
        [Route("{id:int}")]
        public async Task<ResponseViewModel<bool>> DeactivateGymBranch(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.DeactivateBranchAsync(userId, id);
            return ResponseViewModel<bool>.Success(true, "Branche Deactivated successfully.");
        }
        [HttpDelete]
        [Authorize(Roles = "Owner")]
        [Route("{id:int}")]
        public async Task<ResponseViewModel<bool>> DeleteGymBranch(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.DeleteGymBranchAsync(userId, id);
            return ResponseViewModel<bool>.Success(true, "Branche Deleted successfully.");
        }
    }

    }
