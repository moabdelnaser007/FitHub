using FitHubBackendAPI.DTOs.OwnerWallet;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class GymOwnerService: IGymOwnerService
    {
        // DI for repositories and other services would go here
        private readonly IGenericRepository<Visit> _visitRepo;
        private readonly IGenericRepository<GymBranch> _branchRepo;
        private readonly IGenericRepository<OwnerWallet> _ownerWalletRepo;
        private readonly IGenericRepository<Subscription> _subscriptionRepo;
        private readonly IGenericRepository<GymOwner> _gymOwnerRepo;
        public GymOwnerService(IGenericRepository<Visit> visitRepo,
            IGenericRepository<GymBranch> branchRepo,

            IGenericRepository<OwnerWallet> ownerWalletRepo,
            IGenericRepository<Subscription> subscriptionRepo,
            IGenericRepository<GymOwner> gymOwnerRepo)
        {
            _gymOwnerRepo = gymOwnerRepo;
            _branchRepo = branchRepo;
            _ownerWalletRepo = ownerWalletRepo;
            _subscriptionRepo = subscriptionRepo;
            _visitRepo = visitRepo;
        }
        public async Task<int> GetCountBranchVisits(int userId)
        {
            //get branch id that belongs to owner
            //get owner Id from userId
            var owners = await _gymOwnerRepo.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branches = (await _branchRepo.FindAsync(b => b.OwnerId == owner.Id));

            int count = 0;
            if (branches.Count() == 0)
                return 0;

            foreach (var branch in branches)
            {
                var branchId = branch.Id;
                count += (await _visitRepo.FindAsync(v => v.BranchId == branchId)).Count();
                 
            }
            return count;
        }
        public async Task<int> GetCountOwnerSubscriptions(int userId)
        {
            //get owner Id from userId
            var owners = await _gymOwnerRepo.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branches = (await _branchRepo.FindAsync(b => b.OwnerId == owner.Id));
            int count = 0;
            if (branches.Count() == 0)
                return 0;
            foreach (var branch in branches)
            {
                var branchId = branch.Id;
                count += (await _subscriptionRepo.FindAsync(s => s.BranchId == branchId)).Count();
            }
            return count;
        }
        public async Task<int> GetCountOwnerBranches(int userId)
        {
            var owners = await _gymOwnerRepo.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
            var branches = (await _branchRepo.FindAsync(b => b.OwnerId == owner.Id));
            return branches.Count();
        }
        public async Task<ResponseViewModel<OwnerDashBoardDto>> GetOwnerDashboardData(int userId)
        {
            
            var totalBranches = await GetCountOwnerBranches(userId);
            var totalVisits = await GetCountBranchVisits(userId);
            var totalCredits = await _ownerWalletRepo.GetByIdAsync(userId);
            var totalSubscriptions = await GetCountOwnerSubscriptions(userId);
            var dashboardData = new OwnerDashBoardDto
            {
                TotalBranches = totalBranches,
                TotalVisits = totalVisits,
                TotalCredits = totalCredits?.Balance ?? 0,
                TotalSubscriptions = totalSubscriptions
            };
            return ResponseViewModel<OwnerDashBoardDto>.Success(dashboardData);
        }
    }
}
