using FitHubBackendAPI.Data;
using FitHubBackendAPI.DTOs.AuthDTOs;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Services.Interfaces.AuthServices;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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

        public async Task<User> RegisterUserAsync(RegisterUserDto dto)
        { 

            if (dto.Password != dto.ConfirmPassword)
                throw new ValidationException("Passwords do not match.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required.");

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                throw new InvalidOperationException("Email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                City = dto.City,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User,
                Status = AccountStatus.Active
            };
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }


        // ✅ OWNER REGISTER
        public async Task<GymOwner> RegisterOwnerAsync(RegisterOwnerDto dto)
        {
            var ownerwallet = new OwnerWallet
            {
                Balance = 0.0M
            };

            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password mismatch");

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                throw new ValidationException("Email already exists");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                City = dto.City,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Owner,
                Status = AccountStatus.Pending,


            };
            var owner = new GymOwner
            {
                User = user,
                CommercialRegistrationNumber = dto.CommercialRegistrationNumber,
            };
            ownerwallet.User = user;
            owner.Wallet = ownerwallet;
            await _context.GymOwners.AddAsync(owner);
            await _context.SaveChangesAsync();

            return owner;
        }
        //public async Task<GymOwner> RegisterOwnerAsync(RegisterOwnerDto dto)
        //{
        //    if (dto.Password != dto.ConfirmPassword)
        //        throw new ValidationException("Passwords do not match.");

        //    if (string.IsNullOrWhiteSpace(dto.Email))
        //        throw new ValidationException("Email is required.");

        //    if (await _context.GymOwners.AnyAsync(x => x.Email == dto.Email))
        //        throw new InvalidOperationException("Email already exists.");

        //    byte[] licenseBytes;
        //    using (var ms = new MemoryStream())
        //    {
        //        await dto.LicenseFile.CopyToAsync(ms);
        //        licenseBytes = ms.ToArray();
        //    }

        //    var owner = new GymOwner
        //    {
        //        FullName = dto.FullName,
        //        Email = dto.Email,
        //        Phone = dto.Phone,
        //        CommercialRegistrationNumber = dto.CommercialRegistrationNumber,
        //        LicenseFile = licenseBytes,
        //        LicenseFileType = dto.LicenseFile.ContentType,
        //        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        //        Status = AccountStatus.Pending,
        //        ApplicationStatus = ApplicationStatus.PENDING
        //    };
        //    owner.User = user;

            
        //}
        // Register Stuff
        public async Task RegisterStaffAsync(RegisterStaffDTO dto,int ownerId)
        {
            if(ownerId <= 0)
                throw new Exception("Invalid Owner Id");
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password mismatch");
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                throw new ValidationException("Email already exists");
            var owner = await _context.GymOwners.FirstOrDefaultAsync(x => x.UserId == ownerId);
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                City = dto.City,
                Role = UserRole.Staff,
                Status = AccountStatus.Active,
            };
            var staff = new GymStaff
            {
                User = user,
                GymOwnerId = owner.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                Role= "staff",
                Phone = dto.Phone,
                City = dto.City,
                Status = dto.Status
            };
            staff.User = user;

            await _context.GymStaffs.AddAsync(staff);
            await _context.SaveChangesAsync();
        }

        //public async Task<string> LoginAsync(LoginDto dto)
        //{
        //    // =============================
        //    // Try login as normal User
        //    // =============================
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

        //    if (user != null)
        //    {
        //        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        //            throw new UnauthorizedAccessException("Invalid email or password.");

        //        // Auto activate user on first successful login
        //        if (user.Status != AccountStatus.Active)
        //        {
        //            user.Status = AccountStatus.Active;
        //            await _context.SaveChangesAsync();
        //        }

        //        return _jwtService.GenerateToken(user);
        //    }

        //    // =============================
        //    // Try login as Gym Owner
        //    // =============================
        //    var owner = await _context.GymOwners.FirstOrDefaultAsync(x => x.Email == dto.Email);

        //    if (owner != null)
        //    {
        //        if (!BCrypt.Net.BCrypt.Verify(dto.Password, owner.PasswordHash))
        //            throw new UnauthorizedAccessException("Invalid email or password.");

        //        // ❌ لو الأدمن ما وافقش
        //        if (owner.ApplicationStatus != ApplicationStatus.APPROVED)
        //            throw new UnauthorizedAccessException("Your account is pending admin approval.");

        //        // ❌ لو الحساب مش Active
        //        if (owner.Status != AccountStatus.Active)
        //            throw new UnauthorizedAccessException("Your account is not active.");

        //        // ✅ Generate token
        //        var tempUser = new User
        //        {
        //            Id = owner.Id,
        //            Email = owner.Email,
        //            Role = UserRole.Owner
        //        };

        //        return _jwtService.GenerateToken(tempUser);
        //    }

        //    throw new UnauthorizedAccessException("Invalid email or password.");
        //}
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            if (user.Status != AccountStatus.Active)
                throw new Exception("Account not activated wait for admin approval");

            if(user.Role == UserRole.Owner)
            {
                var owner = await _context.GymOwners.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (owner == null || owner.ApplicationStatus != ApplicationStatus.APPROVED)
                    throw new Exception("Your account is pending admin approval.");
            }

            return _jwtService.GenerateToken(user);
        }

        public async Task ReSendOtpAsync(string email)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Email == email);
            if (!userExists)
                throw new KeyNotFoundException("Email not found.");

            // OTP type is ALWAYS ForgotPassword because user initiated this manually
            var otpType = OtpType.ForgotPassword;

            var code = GenerateNumericOtp(6);

            var otp = new VerificationCode
            {
                Email = email,
                Code = code,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                Type = otpType
            };

            await _context.VerificationCodes.AddAsync(otp);
            await _context.SaveChangesAsync();

            await _emailService.SendAsync(
                email,
                "Your FitHub OTP Code",
                $"Your OTP code is: {code}. It will expire in 10 minutes."
            );
        }

        public async Task VerifyOtpAsync(VerifyOtpDto dto)
        {
        var otp = await _context.VerificationCodes
        .FirstOrDefaultAsync(x =>
            x.Email == dto.Email &&
            x.Code == dto.Otp &&
            x.UsedAt == null &&
            x.ExpireAt > DateTime.UtcNow
        );

            if (otp == null)
                throw new ValidationException("Invalid or expired OTP.");

            // ✔️ نعلّم إن الـ OTP اتستخدم
            otp.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null )
                throw new KeyNotFoundException("Email not found.");

            await CreateOtpAndSend(dto.Email, OtpType.ForgotPassword);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new ValidationException("Passwords do not match.");

            // Get the latest OTP for ForgotPassword that matches the code and is not expired.
            var otp = await _context.VerificationCodes
                .Where(x => x.Email == dto.Email
                            && x.Code == dto.Otp
                            && x.Type == OtpType.ForgotPassword
                            && x.ExpireAt > DateTime.UtcNow)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (otp == null)
                throw new ValidationException("Invalid or expired OTP.");

            // If the OTP was previously verified, allow reset. Otherwise, ensure it isn't used.
            if (otp.UsedAt != null)
            {
                // Optional: enforce a small window after verification (e.g., 15 minutes)
                // if ((DateTime.UtcNow - otp.UsedAt.Value) > TimeSpan.FromMinutes(15))
                //     throw new ValidationException("OTP already used.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
                throw new KeyNotFoundException("User/Owner not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // Mark OTP as used at reset time
            otp.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

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

            var html = BuildOtpEmailHtml(code, 10);
            await _emailService.SendAsync(
                email,
                "FitHub OTP Code",
                html,
                isHtml: true
            );
        }


        // helper: secure numeric OTP
        private string GenerateNumericOtp(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            // build digits in a secure way
            var max = (int)Math.Pow(10, length);
            // get random integer in [0, max)
            int value = RandomNumberGenerator.GetInt32(max);
            // pad with leading zeros if needed
            return value.ToString().PadLeft(length, '0');
        }

        private string BuildOtpEmailHtml(string code, int minutes)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>FitHub OTP</title>
  <style>
    body {{
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', Arial, sans-serif;
      background-color: #f5f7fb;
      margin: 0;
      padding: 0;
      color: #213045;
    }}
    .container {{
      max-width: 560px;
      margin: 32px auto;
      background: #ffffff;
      border-radius: 12px;
      box-shadow: 0 8px 24px rgba(33, 48, 69, 0.08);
      overflow: hidden;
      border: 1px solid #e8edf5;
    }}
    .header {{
      background: linear-gradient(135deg, #2b6cb0 0%, #3182ce 100%);
      color: #ffffff;
      padding: 20px 24px;
    }}
    .brand {{
      font-size: 20px;
      font-weight: 700;
      letter-spacing: 0.3px;
    }}
    .content {{
      padding: 24px;
    }}
    .title {{
      font-size: 18px;
      font-weight: 700;
      margin: 0 0 8px;
      color: #213045;
    }}
    .subtitle {{
      font-size: 14px;
      margin: 0 0 16px;
      color: #5b6b83;
    }}
    .code {{
      display: inline-block;
      font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, 'Liberation Mono', 'Courier New', monospace;
      font-size: 24px;
      letter-spacing: 6px;
      font-weight: 800;
      color: #2b6cb0;
      background: #f0f7ff;
      border: 1px solid #cfe2ff;
      border-radius: 10px;
      padding: 16px 20px;
      margin: 12px 0 16px;
    }}
    .note {{
      font-size: 12px;
      color: #6b7a90;
      margin-top: 8px;
    }}
    .footer {{
      padding: 16px 24px;
      font-size: 12px;
      color: #6b7a90;
      background: #fafcff;
      border-top: 1px solid #e8edf5;
      text-align: center;
    }}
    a {{
      color: #2b6cb0;
      text-decoration: none;
    }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <div class=""brand"">FitHub</div>
    </div>
    <div class=""content"">
      <h1 class=""title"">Your One-Time Passcode</h1>
      <p class=""subtitle"">Use the code below to complete your action. For your security, this code will expire in {minutes} minutes.</p>
      <div class=""code"">{code}</div>
      <p class=""note"">If you did not request this code, you can safely ignore this email.</p>
    </div>
    <div class=""footer"">
      © {DateTime.UtcNow.Year} FitHub. All rights reserved.
    </div>
  </div>
</body>
</html>";
}
    }
}
