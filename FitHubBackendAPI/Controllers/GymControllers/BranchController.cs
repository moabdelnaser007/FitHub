using FitHubBackendAPI.DTOs.GymBranchDTOs;

using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

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
        public async Task<ResponseViewModel<GetGymBranchByIdDTO>> CreateBranch(CreateGymBranchDTO dto)
        {

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (userId <= 0)
            {
                return ResponseViewModel<GetGymBranchByIdDTO>.Fail("Invalid user ID.", Entities.Enums.ErrorCode.Unauthorized);
            }

            var branch = await _gymBranchService.CreateGymBranchAsync(userId, dto);
            if (branch == null)
            {
                return ResponseViewModel<GetGymBranchByIdDTO>.Fail("Failed to create gym branch.", Entities.Enums.ErrorCode.BadRequest);
            }
            var createdBranch = new GetGymBranchByIdDTO
            {
                Id = branch.Id,
                //OwnerId = b.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime,
                GenderType = branch.GenderType,
                Status = branch.Status,
                Description = branch.Description,
                WorkingDays = branch.WorkingDays,
                VisitCreditsCost = branch.VisitCreditsCost,
                AmenitiesAvailable = branch.AmenitiesAvailable

            };
            return ResponseViewModel<GetGymBranchByIdDTO>.Success(createdBranch, "Gym branch created successfully.");
        }


        [HttpPut("{Id:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<ResponseViewModel<GetGymBranchByIdDTO>> UpdateBranch(int Id, UpdateGymBranchDTO dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.UpdateGymBranchAsync(userId, dto, Id);
            var branch = await _gymBranchService.GetGymBranchByIdAsync(Id);
            return ResponseViewModel<GetGymBranchByIdDTO>.Success(branch, "Gym branch updated successfully.");
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
        public async Task<ResponseViewModel<bool>> ActivateGymBranch(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.ActivateGymBranchAsync(userId, id);
            return ResponseViewModel<bool>.Success(true, "Branche Activated successfully.");

        }
        [HttpPut]
        [Authorize(Roles = "Owner")]
        public async Task<ResponseViewModel<bool>> DeactivateGymBranch(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.DeactivateBranchAsync(userId, id);
            return ResponseViewModel<bool>.Success(true, "Branche Deactivated successfully.");
        }
        [HttpDelete]
        [Authorize(Roles = "Owner")]
        public async Task<ResponseViewModel<bool>> DeleteGymBranch(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _gymBranchService.DeleteGymBranchAsync(userId, id);
            return ResponseViewModel<bool>.Success(true, "Branche Deleted successfully.");
        }
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ResponseViewModel<bool>> AddImagesToBranch(int branchId, [FromForm] List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
            {
                return ResponseViewModel<bool>.Fail("No images provided.");
            }
            if (branchId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid branch ID.");
            }
            bool result = await _gymBranchService.AddImagesToBranchAsync(branchId, images);
            if (!result)
            {
                return ResponseViewModel<bool>.Fail("Failed to add images to branch.");
            }
            return ResponseViewModel<bool>.Success(result, "Images added to branch successfully.");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResponseViewModel<IEnumerable<GetBranchImagePathDto>>> GetBranchImages(int branchId, string imageName)
        {
            if (branchId <= 0 || string.IsNullOrEmpty(imageName))
            {
                return ResponseViewModel<IEnumerable<GetBranchImagePathDto>>.Fail("Invalid branch ID or image name.");
            }
            var imagePathDto = await _gymBranchService.GetBranchImagesAsync(branchId);
            if (imagePathDto == null)
            {
                return ResponseViewModel<IEnumerable<GetBranchImagePathDto>>.Fail("Image not found.");
            }
            return ResponseViewModel<IEnumerable<GetBranchImagePathDto>>.Success(imagePathDto, "Image path retrieved successfully.");
        }

    }
}
