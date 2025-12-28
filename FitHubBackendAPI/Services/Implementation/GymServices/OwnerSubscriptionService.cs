using AutoMapper;
using FitHubBackendAPI.DTOs.Bookings;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using FitHubBackendAPI.ViewModels;
using System.Linq;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{

    public class OwnerSubscriptionService : IOwnerSubscriptionService
    {
        private readonly IGenericRepository<GymBranch> _branchRepo;
        private readonly IGenericRepository<Subscription> _subscriptionRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;

        public OwnerSubscriptionService(
            IGenericRepository<GymBranch> branchRepo,
            IGenericRepository<Subscription> subscriptionRepo,
            IGenericRepository<Booking> bookingRepo)
        {
            _branchRepo = branchRepo;
            _subscriptionRepo = subscriptionRepo;
            _bookingRepo = bookingRepo;
        }

        // ================= Subscriptions =================
        public async Task<ResponseViewModel<List<SubscriptionForOwnerDto>>> GetOwnerSubscriptionsAsync(int ownerId)
        {
            var branches = await _branchRepo.GetAsync(b => b.OwnerId == ownerId);
            var branchIds = branches.Select(b => b.Id).ToList();

            var subs = await _subscriptionRepo.GetAsync(
                s => branchIds.Contains(s.BranchId),
                includeProperties: "Branch,User,Plan"
            );

            var result = subs.Select(s => new SubscriptionForOwnerDto
            {
                SubscriptionId = s.Id,
                BranchName = s.Branch.BranchName,
                UserName = s.User.FullName,
                PlanName = s.Plan.Name,
                Status = s.Status,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToList();

            return ResponseViewModel<List<SubscriptionForOwnerDto>>.Success(result);
        }

        // ================= Bookings =================
        public async Task<ResponseViewModel<List<BookingForOwnerDto>>> GetOwnerBookingsAsync(int ownerId)
        {
            var branches = await _branchRepo.GetAsync(b => b.OwnerId == ownerId);
            var branchIds = branches.Select(b => b.Id).ToList();

            var bookings = await _bookingRepo.GetAsync(
                b => branchIds.Contains((int)b.BranchId),
                includeProperties: "Branch,User"
            );

            var result = bookings.Select(b => new BookingForOwnerDto
            {
                BookingId = b.Id,
                BranchName = b.Branch.BranchName,
                UserName = b.User.FullName,
                ScheduledDate = (DateTime)b.ScheduledDateTime,
                Status = b.Status,
                CreditsCost = b.CreditsCost
            }).ToList();

            return ResponseViewModel<List<BookingForOwnerDto>>.Success(result);
        }
    }


}
