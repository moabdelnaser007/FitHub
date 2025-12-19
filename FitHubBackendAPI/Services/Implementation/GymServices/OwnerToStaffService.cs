using AutoMapper;
using FitHubBackendAPI.DTOs.StuffDTOs;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.GymBranch;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class OwnerToStaffService : IOwnerToStaffService
    {
        private readonly IGenericRepository<GymStaff> _staffMemberRepository;
        private readonly IMapper _mapper;

        public OwnerToStaffService(IGenericRepository<GymStaff> staffMemberRepository, IMapper mapper)
        {
            _staffMemberRepository = staffMemberRepository;
        }
        public async Task<GetStuffDTO> GetStaffMemberByIdAsync(int staffId)
        {
              var staffMember= await _staffMemberRepository.GetByIdAsync(staffId);
              //return new GetStuffDTO
              //{
              //    Id = staffMember.Id,
              //    FullName = staffMember.FullName,
              //    Email = staffMember.Email,
              //    Phone = staffMember.Phone,
              //    BranchId = staffMember.BranchId,
              //    Status = staffMember.Status
              //};
              return _mapper.Map<GetStuffDTO>(staffMember);
        }
        public async Task<IEnumerable<GetStuffDTO>> GetAllStaffMembersAsync(int branchId)
        {
            var staffMembers = (await _staffMemberRepository.GetAllAsync()).Where(s => s.BranchId == branchId);
            return _mapper.Map<IEnumerable<GetStuffDTO>>(staffMembers.ToList());
        }
        public async Task<UpdateStaffDTO> UpdateStaff(UpdateStaffDTO dto)
        {
            var staffMember = await _staffMemberRepository.GetByIdAsync(dto.Id);
            if (staffMember == null) return null;

            _mapper.Map(dto, staffMember);
            _staffMemberRepository.Update(staffMember);
            await _staffMemberRepository.SaveChangesAsync();

            return _mapper.Map<UpdateStaffDTO>(staffMember);
        }

        public async Task<bool> AssignStaffToBranch(int staffId, int branchId)
        {
            var member =await _staffMemberRepository.GetByIdAsync(staffId);
            member.BranchId = branchId;
            _staffMemberRepository.Update(member);
            await _staffMemberRepository.SaveChangesAsync();
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
