using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.DTOs.Reviews;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;

using FitHubBackendAPI.Services.Interfaces.OwnerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace FitHubBackendAPI.Services.Implementation.GymServices
{
    public class GymBranchService : IGymBranchService
    {

        private readonly IGenericRepository<GymBranch> _repository;
        private readonly IGenericRepository<GymOwner> _ownerRepository;
        private readonly IGenericRepository<Image> _imagesRepository;
        private readonly IGenericRepository<Review> _reviewRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GymBranchService(IGenericRepository<GymBranch> repository,
            IGenericRepository<GymOwner> ownerRepository,
            IGenericRepository<Image> imagesRepository, 
            IGenericRepository<Review> reviewRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _ownerRepository = ownerRepository;
            _repository = repository;
            _imagesRepository = imagesRepository;
            _webHostEnvironment = webHostEnvironment;
            _reviewRepository = reviewRepository;
        }
        public async Task<Entities.Models.GymBranch> CreateGymBranchAsync(int userId, CreateGymBranchDTO dto)
        {
            /*
             -list ifile
            -dto of create
            -create gym
            -get gymid
            -handle images and save them with gymid
            -save image entity
             
             */
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");

            Entities.Models.GymBranch branch = new Entities.Models.GymBranch
            {
                OwnerId = owner.Id,
                BranchName = dto.BranchName,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                OpenTime = dto.OpenTime,
                CloseTime = dto.CloseTime,
                GenderType = dto.GenderType,
                Status = dto.Status,
                Description = dto.Description,
                WorkingDays = dto.WorkingDays,
                VisitCreditsCost = dto.VisitCreditsCost,
                AmenitiesAvailable = dto.AmenitiesAvailable,
                rating = 0
            };
            
            await _repository.AddAsync(branch);



            await _repository.SaveChangesAsync();

            return branch;
        }
        //get all branches for all users Except is deleted or inactive
        public async Task<IEnumerable<GetAllBranchDTO>> GetAllActiveBranchesAsync()
        {
            var branches = await _repository.GetAllAsync();
            return branches.Where(b => b.IsAcTive == true).Include(g=>g.Images)
             .Select(b => new GetAllBranchDTO
             {
                 Id = b.Id,
                 //OwnerId = b.OwnerId,
                 BranchName = b.BranchName,
                 Phone = b.Phone,
                 Address = b.Address,
                 City = b.City,
                 rating = b.rating,
                 OpenTime = b.OpenTime,
                 CloseTime = b.CloseTime,
                 GenderType = b.GenderType,
                 Status = b.Status,
                 Description = b.Description,
                 WorkingDays = b.WorkingDays,
                 VisitCreditsCost = b.VisitCreditsCost,
                 AmenitiesAvailable = b.AmenitiesAvailable,
                 Images = b.Images.Select(img => new GetBranchImagePathDto
                 {
                     imageName = img.imageName,
                     imagePath = img.imagePath
                 }).ToList()
             }).ToList();
        }
        //get a branch by id if it's active
        public async Task<GetGymBranchByIdDTO> GetActiveGymBranchByIdAsync(int branchId)
        {
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null || branch.IsAcTive != true)
                throw new Exception("Branch not found or inactive");
            //get reviews of this branch
            var reviews = await _reviewRepository.GetAsync(r => r.BranchId == branchId,
                includeProperties:"User");
            var imgs = await GetBranchImagesAsync(branchId);
            return new GetGymBranchByIdDTO
            {
                Id = branch.Id,
                //OwnerId = b.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                rating = branch.rating,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime,
                GenderType = branch.GenderType,
                Status = branch.Status,
                Description = branch.Description,
                WorkingDays = branch.WorkingDays,
                VisitCreditsCost = branch.VisitCreditsCost,
                AmenitiesAvailable = branch.AmenitiesAvailable,
                Images = imgs.ToList(),
                Reviews = reviews.Select(r => new GetAllBranchRevewsDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserName = r.User.FullName
                }).ToList()
            };
        }

        public async Task DeactivateBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");

            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.Status = BranchStatus.INACTIVE;
            _repository.Update(branch);
            await _repository.SaveChangesAsync();


        }

        public async Task<IEnumerable<GetAllBranchDTO>> GetAllBranchesAsync(int userId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");
            
            var branches = _repository.GetAllAsync().Result
                .Where(b => b.OwnerId == owner.Id).Include(g=>g.Images)
                .Select(b => new GetAllBranchDTO
                {
                    Id = b.Id,
                    //OwnerId = b.OwnerId,
                    BranchName = b.BranchName,
                    Phone = b.Phone,
                    Address = b.Address,
                    City = b.City,
                    OpenTime = b.OpenTime,
                    CloseTime = b.CloseTime,
                    GenderType = b.GenderType,
                    Status = b.Status,
                    Description = b.Description,
                    WorkingDays = b.WorkingDays,
                    VisitCreditsCost = b.VisitCreditsCost,
                    AmenitiesAvailable = b.AmenitiesAvailable,
                    Images = b.Images.Select(img => new GetBranchImagePathDto
                    {
                        imageName = img.imageName,
                        imagePath = img.imagePath
                    }).ToList()
                });
            return await branches.ToListAsync();
        }

        public async Task<GetGymBranchByIdDTO> GetGymBranchByIdAsync(int branchId)
        {
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
                throw new Exception("Branch not found");
            if (branch.IsAcTive == false)
                throw new Exception("Branch is Suspended");
            var imgs = await GetBranchImagesAsync(branchId);
            return new GetGymBranchByIdDTO
            {
                Id = branch.Id,
                //OwnerId = b.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime,
                GenderType = branch.GenderType,
                Status = branch.Status,
                Description = branch.Description,
                WorkingDays = branch.WorkingDays,
                VisitCreditsCost = branch.VisitCreditsCost,
                AmenitiesAvailable = branch.AmenitiesAvailable,
                Images = imgs.ToList()

            };
        }

        public async Task UpdateGymBranchAsync(int userId, UpdateGymBranchDTO dto, int BranchId)
        {
            // 1️⃣ هات الـ Owner
            var owner = (await _ownerRepository.FindAsync(o => o.UserId == userId))
                        .FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");

            // 2️⃣ هات الفرع
            var branch = await _repository.GetByIdAsync(BranchId);

            if (branch == null)
                throw new Exception("Branch not found");

            // 3️⃣ تأكد إن الفرع تابع لنفس الـ Owner
            if (branch.OwnerId != owner.Id)
                throw new Exception("You are not authorized to update this branch");

            // 4️⃣ Update
            if (branch.IsAcTive)
            {
                branch.BranchName = dto.BranchName;
                branch.Phone = dto.Phone;
                branch.Address = dto.Address;
                branch.City = dto.City;
                branch.OpenTime = dto.OpenTime;
                branch.CloseTime = dto.CloseTime;
                branch.GenderType = dto.GenderType;
                branch.Status = dto.Status;
                branch.Description = dto.Description;
                branch.WorkingDays = dto.WorkingDays;
                branch.VisitCreditsCost = dto.VisitCreditsCost;
                branch.AmenitiesAvailable = dto.AmenitiesAvailable;

                branch.UpdatedAt = DateTime.UtcNow;

                _repository.Update(branch);
                await _repository.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cannot update a suspended branch");
            }

        }
        public async Task ActivateGymBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");

            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            if (branch.IsAcTive == false)
            {
                throw new Exception("Cannot activate a Suspended branch");
            }
            branch.Status = BranchStatus.ACTIVE;
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
        }
        public async Task DeleteGymBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();

            if (owner == null)
                throw new Exception("Owner not found");

            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to delete this branch");
            }
            await _repository.SoftDelete(branch);
            await _repository.SaveChangesAsync();
        }
        public async Task<bool> AddImagesToBranchAsync(int branchId, List<IFormFile> images)
        {
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string folderPath = Path.Combine(wwwRootPath, "images\\Gym", branch.BranchName);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(folderPath);
            foreach (var image in images)
            {

                if (image != null)
                {
                    var fileName = $"{Guid.NewGuid().ToString()}-{branch.BranchName}" + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(folderPath, fileName);


                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    Image imageEntity = new Image
                    {
                        imageName = fileName,
                        imagePath = filePath,
                        branch = branch
                    };
                    branch.Images.Add(imageEntity);
                }

            }
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
            if (branch.Images.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<IEnumerable<GetBranchImagePathDto>> GetBranchImagesAsync(int branchId)
        {
            var branch = await _repository.IsExist(branchId);
            if (!branch )
            {
                throw new Exception("Branch not found");
            }
            var images = await _imagesRepository.FindAsync(img => img.GymId == branchId);
            //if (images == null || images.Count() == 0)
            //{
            //    throw new Exception("No images found for this branch");
            //}
            var dtos = images.Select(image => new GetBranchImagePathDto
            {
                imageName = image.imageName,
                imagePath = image.imagePath
            });
            return dtos;
        }
        public async Task<bool> RemoveImageFromBranchAsync(int branchId, string imageName)
        {
            var query = await _repository.GetAsync(b=>b.Id==branchId,
                                        includeProperties:"Images");
            var branch = query.FirstOrDefault();
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            var image = branch.Images.FirstOrDefault(img => img.imageName == imageName);
            if (image == null)
            {
                throw new Exception("Image not found in this branch");
            }
            // Remove the image file from the server
            if (File.Exists(image.imagePath))
            {
                File.Delete(image.imagePath);
            }
            // Remove the image entity from the branch
            branch.Images.Remove(image);
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
            return true;
        }
        public async Task<GetBranchImagePathDto> GetBranchImagePathAsync(int branchId, string imageName)
        {
            var query = await _repository.GetAsync(b => b.Id == branchId,
                                        includeProperties: "Images");
            var branch = query.FirstOrDefault();
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            var image = branch.Images.FirstOrDefault(img => img.imageName == imageName);
            if (image == null)
            {
                throw new Exception("Image not found in this branch");
            }
            return new GetBranchImagePathDto
            {
                imageName = image.imageName,
                imagePath = image.imagePath
            };
        }
        public async Task<bool> SetCoverImage(string imageName,int branchId)
        {
            var image = await GetBranchImagePathAsync(branchId, imageName);
            var branch =  await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            branch.CoverImagePath = image.imagePath;
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
            return true;
        }
        }
}
