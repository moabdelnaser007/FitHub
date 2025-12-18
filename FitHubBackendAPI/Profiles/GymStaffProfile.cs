using AutoMapper;
using FitHubBackendAPI.DTOs.StuffDTOs;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Profiles
{
    public class GymStaffProfile:Profile
    {
        public GymStaffProfile()
        {
            CreateMap<GymStaff, GetStuffDTO>();
            CreateMap<GymStaff, UpdateStaffDTO>().ReverseMap();
        }
    }
}
