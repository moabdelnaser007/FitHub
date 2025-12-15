using FitHubBackendAPI.DTOs.PlanDTOs;
using AutoMapper;
using FitHubBackendAPI.Services.Interfaces.GymBranch;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class PlanService : IPlanService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<GymPlan> _planRepository;
        public PlanService(IMapper mapper, IGenericRepository<GymPlan> planRepository)
        {
            _mapper = mapper;
            _planRepository = planRepository;
        }
        public async Task ActivatePlanAsync(int planId)
        {
            var plan = await _planRepository.GetByIdAsync(planId);
            if (plan == null) return;
            plan.Status = PlanStatus.ACTIVE;
            _planRepository.Update(plan);
        }

        public async Task<GetPlanByIdDTO> CreatePlanAsync(CreatePlanDTO createPlanDto)
        {
            var plan = _mapper.Map<GymPlan>(createPlanDto);
            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();
            return new GetPlanByIdDTO();
        }

        public async Task DeactivatePlanAsync(int planId)
        {
            var plan = await _planRepository.GetByIdAsync(planId);
            if (plan == null) return;
            plan.Status = PlanStatus.INACTIVE;
            _planRepository.Update(plan);
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            var plan = await _planRepository.GetByIdAsync(planId);
            if (plan == null) return false;
            bool delete = await _planRepository.SoftDelete(plan);
            await _planRepository.SaveChangesAsync();
            return delete;

        }

        public async Task<GetPlanByIdDTO> GetPlanByIdAsync(int planId)
        {
            var plan = await _planRepository.GetByIdAsync(planId);
            return _mapper.Map<GetPlanByIdDTO>(plan);
        }

        public async Task<IEnumerable<GetPlanByBranchIdDTO>> GetPlansByBranchIdAsync(int branchId)
        {
            var query = await _planRepository.GetAllAsync();
            var plans = query.Where(p => p.BranchId == branchId);
            return _mapper.Map<IEnumerable<GetPlanByBranchIdDTO>>(plans);
        }

        public async Task<GetPlanByIdDTO> UpdatePlanAsync(UpdatePlanDTO updatePlanDto)
        {
            var plan = await _planRepository.GetByIdAsync(updatePlanDto.Id);
            if (plan == null) return null;

            var updatePlan = _mapper.Map(updatePlanDto, plan);
            _planRepository.Update(updatePlan);
            await _planRepository.SaveChangesAsync();
            return _mapper.Map<GetPlanByIdDTO>(updatePlan);
        }
    }
}
