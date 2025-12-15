using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.GymBranch;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class OwnerToStaffService : IOwnerToStaffService
    {
        private readonly IGenericRepository<GymStaff> _staffMemberRepository;
        public OwnerToStaffService(IGenericRepository<GymStaff> staffMemberRepository)
        {
            _staffMemberRepository = staffMemberRepository;
        }

        public async Task<bool> AssignStaffToBranch(int staffId, int branchId)
        {
            var member =await _staffMemberRepository.GetByIdAsync(staffId);
            member.BranchId = branchId;
            _staffMemberRepository.Update(member);
            return true;
        }

        public async Task<bool> DeleteStaffMemberAsync(int staffId)
        {
            var staffMember = await _staffMemberRepository.GetByIdAsync(staffId);
            if (staffMember == null) return false;

            await _staffMemberRepository.SoftDelete(staffMember);
            await _staffMemberRepository.SaveChangesAsync();

            return !(await _staffMemberRepository.IsExist(staffId));
        }
    }
}
