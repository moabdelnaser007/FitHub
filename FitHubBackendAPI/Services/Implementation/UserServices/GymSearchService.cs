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

            // search by name
            if (!string.IsNullOrWhiteSpace(query.Name))
                gymsQuery = gymsQuery.Where(x => x.BranchName!.Contains(query.Name));

            // filter by city
            if (!string.IsNullOrWhiteSpace(query.City))
                gymsQuery = gymsQuery.Where(x => x.City == query.City);

            // filter by rating
            if (query.MinRating.HasValue)
                gymsQuery = gymsQuery.Where(x =>
                    x.Reviews.Any() &&
                    x.Reviews.Average(r => r.Rating) >= query.MinRating.Value
                );

            // 👈 هنا نطلع الداتا من الداتابيز
            var gyms = await gymsQuery
                .Select(x => new
                {
                    x.Id,
                    x.BranchName,
                    //x.CoverImageUrl,
                    x.City,
                    x.Address,
                    x.AmenitiesAvailable,
                    Ratings = x.Reviews.Select(r => r.Rating)
                })
                .ToListAsync();

            return gyms.Select(x => new GymSearchResultDto
            {
                Id = x.Id,
                Name = x.BranchName!,
                //Image = x.CoverImageUrl!,
                Address = $"{x.City}, {x.Address}",

                Rating = x.Ratings.Any()
            ? (decimal)Math.Round((decimal)x.Ratings.Average(), 1)
            : 0m,

                Amenities = x.AmenitiesAvailable.HasValue
            ? Enum.GetValues(typeof(GymAmenity))
                .Cast<GymAmenity>()
                .Where(a => a != 0 && x.AmenitiesAvailable.Value.HasFlag(a))
                .Select(a => a.ToString())
                .Take(2)
                .ToList()
            : new List<string>()
            }).ToList();
        }

    }
}
