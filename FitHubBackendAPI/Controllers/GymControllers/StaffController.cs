using AutoMapper;
using FitHubBackendAPI.DTOs.StuffDTOs;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Implementation;
using FitHubBackendAPI.Repository.Interfaces;
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
    [Authorize(Roles = "Owner")]
    public class StaffController : ControllerBase
    {
        private readonly IOwnerToStaffService _ownerToStaffService;
        private readonly IMapper _mapper;
        public StaffController(IOwnerToStaffService staffMemberRepository,IMapper mapper)
        {
            _ownerToStaffService = staffMemberRepository;
            _mapper = mapper; 
        }

        [HttpGet]
        [Route("GetStaffMembers/{branchId}")]
        public async Task<ResponseViewModel<IEnumerable<GetStuffDTO>>> GetAllBranchStaff(int branchId)
        {
            var members = await _ownerToStaffService.GetAllStaffMembersAsync(branchId);
            if (members.Count() == 0)
            {
                return ResponseViewModel<IEnumerable<GetStuffDTO>>.Fail("no Staff member in this Gym", Entities.Enums.ErrorCode.NoContent);
            }
            return ResponseViewModel<IEnumerable<GetStuffDTO>>.Success(members, "Staff members retreved successfully");
        }
        [HttpGet]
        [Route("GetStaffMember/{staffId}")]
        public async Task<ResponseViewModel<GetStuffDTO>> GetStaffMember(int staffId)
        {
            var members = await _ownerToStaffService.GetStaffMemberByIdAsync(staffId);
            if (members == null)
            {
                return ResponseViewModel<GetStuffDTO>.Fail("no Staff member not Found", Entities.Enums.ErrorCode.NotFound);
            }
            return ResponseViewModel<GetStuffDTO>.Success(members, "Staff members retreved successfully");
        }

        [HttpDelete]
        [Route("DeleteStaffMember/{staffId}")]
        public async Task<ResponseViewModel<bool>> DeleteStaffMember(int staffId)
        {
            if (staffId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid staff member ID.");
            }
            var res = await _ownerToStaffService.DeleteStaffMemberAsync(staffId);

            return ResponseViewModel<bool>.Success(res, "Staff member deleted successfully");
        }
        [HttpGet]
        [Route("AssignStaffToBranch/{staffId}/{branchId}")]
        public async Task<ResponseViewModel<bool>> AssignStaffToBranch(int staffId, int branchId)
        {
            if (staffId <= 0 || branchId <= 0)
            {
                return ResponseViewModel<bool>.Fail("Invalid staff or branch ID.");
            }
            var res = await _ownerToStaffService.AssignStaffToBranch(staffId, branchId);

            return ResponseViewModel<bool>.Success(res, "Staff member assigned to branch successfully");
        }
    }
}
