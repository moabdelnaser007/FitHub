using AutoMapper;
using FitHubBackendAPI.DTOs.AdminDtos;
using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Profiles
{
    public class AdminOwnerProfile:Profile
    {
        public AdminOwnerProfile()
        {
            CreateMap<GymOwner, GetOwnerForAdminDto>();
            CreateMap<User, GetUserDataDto>();
            CreateMap<Subscription, GetUserSubscriptionDataDto>();
            CreateMap<Review, GetUserReviewDto>();
        }
    }
}
