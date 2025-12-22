using FitHubBackendAPI.DTOs.GymBranchDTOs;
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
        private readonly IGenericRepository<GymAmenities> _amenitiesRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GymBranchService(IGenericRepository<GymBranch> repository, IGenericRepository<GymOwner> ownerRepository, IGenericRepository<GymAmenities> amenitiesRepository, IWebHostEnvironment webHostEnvironment)
        {
            _ownerRepository = ownerRepository;
            _repository = repository;
            _amenitiesRepository = amenitiesRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<Entities.Models.GymBranch> CreateGymBranchAsync(int userId, CreateGymBranchDTO dto,List<IFormFile>images)
        {
            /*
             -list ifile
            -dto of create
            -create gym
            -get gymid
            -handle images and save them with gymid
            -save image entity
             
             */
            var owners = await _ownerRepository.FindAsync(o=>o.UserId==userId);
            var owner = owners.FirstOrDefault();
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
                VisitCreditsCost = dto.VisitCreditsCost
            };
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string folderPath = Path.Combine(wwwRootPath,"images\\Gym",branch.BranchName);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(folderPath);
            foreach (var image in images)
            {
                
                if (image != null)
                {
                    var fileName = $"{Guid.NewGuid().ToString()}-{branch.BranchName}" + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(folderPath,fileName);
                    

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    Image imageEntity = new Image
                    {
                        imagePath = filePath,
                        branch = branch
                    };
                    branch.Images.Add(imageEntity);
                }
                
            }
            await _repository.AddAsync(branch);

            
                
            await _repository.SaveChangesAsync();

            return branch;
        }
        //get all branches for all users Except is deleted or inactive
        public async Task<IEnumerable<GetAllBranchDTO>> GetAllActiveBranchesAsync()
        {
            var branches =await  _repository.GetAllAsync();
               return branches.Where(b => b.IsAcTive  == true)
                .Select(b => new GetAllBranchDTO
                {
                    //Id = b.Id,
                    //OwnerId = b.OwnerId,
                    BranchName = b.BranchName,
                    Phone = b.Phone,
                    Address = b.Address,
                    City = b.City,
                    OpenTime = b.OpenTime,
                    CloseTime = b.CloseTime,

                }).ToList();
        }
        //get a branch by id if it's active
        public async Task<GetGymBranchByIdDTO> GetActiveGymBranchByIdAsync(int branchId)
        {
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null || branch.IsAcTive != true)
                throw new Exception("Branch not found or inactive");
            return new GetGymBranchByIdDTO
            {
                //Id = branch.Id,
                //OwnerId = branch.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime
                };
        }

        public async Task DeactivateBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
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
            var branches = _repository.GetAllAsync().Result
                .Where(b => b.OwnerId == owner.Id)
                .Select(b => new GetAllBranchDTO
                {
                    //Id = b.Id,
                    //OwnerId = b.OwnerId,
                    BranchName = b.BranchName,
                    Phone = b.Phone,
                    Address = b.Address,
                    City = b.City,
                    OpenTime = b.OpenTime,
                    CloseTime = b.CloseTime,
                    GenderType = b.GenderType,
                    Status = b.Status
                });
            return await branches.ToListAsync();
        }

        public async Task<GetGymBranchByIdDTO> GetGymBranchByIdAsync(int branchId)
        {
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
                throw new Exception("Branch not found");
            if(branch.IsAcTive == false)
                throw new Exception("Branch is Suspended");
            return new GetGymBranchByIdDTO
            {
                //Id = branch.Id,
                //OwnerId = branch.OwnerId,
                BranchName = branch.BranchName,
                Phone = branch.Phone,
                Address = branch.Address,
                City = branch.City,
                OpenTime = branch.OpenTime,
                CloseTime = branch.CloseTime,
                GenderType = branch.GenderType,
                Status = branch.Status

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
            if(branch.IsAcTive)
            {
                branch.BranchName = dto.BranchName;
                branch.Phone = dto.Phone;
                branch.Address = dto.Address;
                branch.City = dto.City;
                branch.OpenTime = dto.OpenTime;
                branch.CloseTime = dto.CloseTime;
                branch.GenderType = dto.GenderType;
                branch.Status = dto.Status;
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
            var branch = await _repository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            if (branch.OwnerId != owner.Id)
            {
                throw new Exception("You are not authorized to update this branch");
            }
            branch.Status = BranchStatus.ACTIVE;
            _repository.Update(branch);
            await _repository.SaveChangesAsync();
        }
        public async Task DeleteGymBranchAsync(int userId, int branchId)
        {
            var owners = await _ownerRepository.FindAsync(o => o.UserId == userId);
            var owner = owners.FirstOrDefault();
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
    }
}
