using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.AuthDTOs;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Enums;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;
using FitHubBackendAPI.Services.Interfaces.AuthServices;
using FitHubBackendAPI.Entities.Models;


namespace FitHubBackendAPI.Services.Implementation.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly FitHubDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(FitHubDbContext context, IJwtService jwtService, IEmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        // USER REGISTER
        public async Task RegisterUserAsync(RegisterUserDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password mismatch");

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                throw new Exception("Email already exists");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                City = dto.City,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User,
                Status = AccountStatus.Pending
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await CreateOtpAndSend(dto.Email, OtpType.Register);
        }

        // OWNER REGISTER
        public async Task RegisterOwnerAsync(RegisterOwnerDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password mismatch");

            var owner = new GymOwner
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CommercialRegistrationNumber = dto.CommercialRegistrationNumber,
                LicenseFileUrl = "uploaded/path",
                Status = AccountStatus.Pending
            };

            await _context.GymOwners.AddAsync(owner);
            await _context.SaveChangesAsync();

            await CreateOtpAndSend(dto.Email, OtpType.Register);

        }

        // LOGIN
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            if (user.Status != AccountStatus.Active)
                throw new Exception("Account not activated");

            return _jwtService.GenerateToken(user);
        }

        // SEND OTP 
        public async Task SendOtpAsync(SendOtpDto dto)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            var ownerExists = await _context.GymOwners.AnyAsync(x => x.Email == dto.Email);

            if (!userExists && !ownerExists)
                throw new Exception("Email not found");

            var code = new Random().Next(100000, 999999).ToString();

            var otp = new VerificationCode
            {
                Email = dto.Email,
                Code = code,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                Type = dto.Type
            };

            await _context.VerificationCodes.AddAsync(otp);
            await _context.SaveChangesAsync();

            await _emailService.SendAsync(
                dto.Email,
                "OTP Code",
                $"Your OTP is: {code}"
            );
        }


        // VERIFY OTP
        public async Task VerifyOtpAsync(VerifyOtpDto dto)
        {
            var otp = await _context.VerificationCodes
                .FirstOrDefaultAsync(x => x.Email == dto.Email
                    && x.Code == dto.Otp
                    && x.UsedAt == null
                    && x.ExpireAt > DateTime.UtcNow);

            if (otp == null)
                throw new Exception("Invalid OTP");

            otp.UsedAt = DateTime.UtcNow;

            var user = await _context.Users.FirstAsync(x => x.Email == dto.Email);
            user.Status = AccountStatus.Active;

            await _context.SaveChangesAsync();
        }

        // FORGOT PASSWORD
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
                throw new Exception("User not found");

            await CreateOtpAndSend(dto.Email, OtpType.ForgotPassword);
        }

        // RESET PASSWORD
        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var otp = await _context.VerificationCodes
                .FirstOrDefaultAsync(x => x.Email == dto.Email
                    && x.Code == dto.Otp
                    && x.Type == OtpType.ForgotPassword
                    && x.UsedAt == null);

            if (otp == null)
                throw new Exception("Invalid OTP");

            var user = await _context.Users.FirstAsync(x => x.Email == dto.Email);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            otp.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // OTP GENERATION
        private async Task CreateOtpAndSend(string email, OtpType type)
        {
            var code = new Random().Next(100000, 999999).ToString();

            var otp = new VerificationCode
            {
                Email = email,
                Code = code,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                Type = type
            };

            await _context.VerificationCodes.AddAsync(otp);
            await _context.SaveChangesAsync();

            await _emailService.SendAsync(email, "OTP Code", $"Your OTP is: {code}");
        }
    }
}
