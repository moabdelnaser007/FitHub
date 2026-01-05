using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class GymSearchService : IGymSearchService
    {
        private readonly FitHubDbContext _context;

        public GymSearchService(FitHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<GymSearchResultDto>> SearchAsync(GymSearchQueryDto query)
        {
            var gymsQuery = _context.GymBranches
                .Include(x => x.Reviews)
                .AsQueryable();

            // 🔍 Name
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                gymsQuery = gymsQuery.Where(x =>
                    x.BranchName != null &&
                    x.BranchName.Contains(query.Name));
            }

            // 🏙️ City
            if (!string.IsNullOrWhiteSpace(query.City))
            {
                gymsQuery = gymsQuery.Where(x => x.City == query.City);
            }

            // 📍 Address
            if (!string.IsNullOrWhiteSpace(query.Address))
            {
                gymsQuery = gymsQuery.Where(x =>
                    x.Address != null &&
                    x.Address.Contains(query.Address));
            }

            // ⭐ Rating
            if (query.MinRating.HasValue)
            {
                gymsQuery = gymsQuery.Where(x =>
                    x.Reviews.Any() &&
                    x.Reviews.Average(r => r.Rating) >= query.MinRating.Value);
            }

            // 💰 Max Visit Credits
            if (query.MaxVisitCredits.HasValue)
            {
                gymsQuery = gymsQuery.Where(x =>
                    x.VisitCreditsCost <= query.MaxVisitCredits.Value);
            }

            // 🏷️ Amenities (Flags)
            if (query.Amenities.HasValue)
            {
                gymsQuery = gymsQuery.Where(x =>
                    x.AmenitiesAvailable.HasValue &&
                    (x.AmenitiesAvailable.Value & query.Amenities.Value) == query.Amenities.Value
                );
            }

            // ⬇️ تحميل الداتا من DB
            var gyms = await gymsQuery
                .Select(x => new
                {
                    x.Id,
                    x.BranchName,
                    x.City,
                    x.Address,
                    x.VisitCreditsCost,
                    x.AmenitiesAvailable,
                    Ratings = x.Reviews.Select(r => r.Rating)
                })
                .ToListAsync();

            // ⬇️ Mapping بعد DB
            return gyms.Select(x => new GymSearchResultDto
            {
                Id = x.Id,
                Name = x.BranchName!,
                City = x.City,
                Address = $"{x.City}, {x.Address}",
                VisitCreditsCost = (int?)x.VisitCreditsCost,
                Amenities = x.AmenitiesAvailable,

                Rating = x.Ratings.Any()
                    ? Math.Round((decimal)x.Ratings.Average(), 1)
                    : 0m
            }).ToList();
        }

    }
}
