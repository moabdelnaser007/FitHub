using AutoMapper;
using FitHubBackendAPI.DTOs.PlanDTOs;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Profiles
{
    public class GymPlanProfile: Profile
    {
        public GymPlanProfile() 
        {
            CreateMap<CreatePlanDTO, GymPlan>();
            CreateMap<GymPlan, GetPlanByIdDTO>();
            CreateMap<GymPlan, GetPlanByBranchIdDTO>();
            CreateMap<UpdatePlanDTO, GymPlan>();
        }
    }
}
