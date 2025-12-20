using AutoMapper;
using FitHubBackendAPI.DTOs.StuffDTOs;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.GymBranch;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class OwnerToStaffService : IOwnerToStaffService
    {
        private readonly IGenericRepository<GymStaff> _staffMemberRepository;
        private readonly IGenericRepository<GymOwner> _gymOwnerRepository;
        private readonly IMapper _mapper;

        public OwnerToStaffService(IGenericRepository<GymStaff> staffMemberRepository, IGenericRepository<GymOwner> gymOwnerRepository, IMapper mapper)
        {
            _staffMemberRepository = staffMemberRepository;
            _gymOwnerRepository = gymOwnerRepository;
            _mapper = mapper;
        }
        public async Task<GetStuffDTO> GetStaffMemberByIdAsync(int staffId)
        {
            var staffMember = await _staffMemberRepository.GetByIdAsync(staffId);

            if (staffMember == null)
                return null;

            return _mapper.Map<GetStuffDTO>(staffMember);
        }
        public async Task<IEnumerable<GetStuffDTO>> GetAllStaffAsync(int ownerId)
        {
            var gymOwner = await _gymOwnerRepository.FindAsync(o => o.UserId == ownerId);
            var staffMembers = await _staffMemberRepository
                .FindAsync(s => s.GymOwnerId == gymOwner.FirstOrDefault().Id);

            if (staffMembers == null || !staffMembers.Any())
                return new List<GetStuffDTO>();

            return staffMembers.Select(s => new GetStuffDTO
            {
                Id = s.Id,
                UserId = s.UserId,
                BranchId = s.BranchId,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                Role = s.Role,
                Status = s.Status
            }).ToList();
        }

        public async Task<IEnumerable<GetStuffDTO>> GetAllStaffMembersAsync(int branchId)
        {
            var staffMembers = await _staffMemberRepository
                .FindAsync(s => s.BranchId == branchId);

            if (staffMembers == null || !staffMembers.Any())
                return new List<GetStuffDTO>();

            return staffMembers.Select(s => new GetStuffDTO
            {
                Id = s.Id,
                UserId = s.UserId,
                BranchId = s.BranchId,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                Role = s.Role,
                
                Status = s.Status
            }).ToList();
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
