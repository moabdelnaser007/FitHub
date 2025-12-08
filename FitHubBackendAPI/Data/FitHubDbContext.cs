using FitHubBackendAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FitHubBackendAPI.Data
{
    public class FitHubDbContext : DbContext
    {
        public FitHubDbContext(DbContextOptions<FitHubDbContext> options)
            : base(options)
        {
        }

        // ===== DbSets =====
        public DbSet<GymOwner> GymOwners { get; set; }
        public DbSet<OwnerWallet> OwnerWallets { get; set; }
        public DbSet<GymBranch> GymBranches { get; set; }
        public DbSet<GymStaff> GymStaffs { get; set; }
        public DbSet<GymAmenities> GymAmenities { get; set; }
        public DbSet<GymPlan> GymPlans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<UserCreditTransactions> UserCreditTransactions { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OwnerSettlement> OwnerSettlements { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============================
            // ✅ ENUM CONVERSIONS
            // =============================

            modelBuilder.Entity<GymOwner>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<GymOwner>().Property(x => x.ApplicationStatus).HasConversion<string>();

            modelBuilder.Entity<GymBranch>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<GymBranch>().Property(x => x.GenderType).HasConversion<string>();

            modelBuilder.Entity<GymStaff>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<GymPlan>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<User>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<UserCreditTransactions>().Property(x => x.TransactionType).HasConversion<string>();
            modelBuilder.Entity<UserCreditTransactions>().Property(x => x.Source).HasConversion<string>();

            modelBuilder.Entity<Subscription>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<Booking>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<Visit>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<OwnerSettlement>().Property(x => x.PayoutStatus).HasConversion<string>();

            modelBuilder.Entity<VerificationCode>().Property(x => x.Type).HasConversion<string>();


            // =============================
            // ✅ TIMESPAN CONVERSION
            // =============================

            modelBuilder.Entity<GymBranch>()
                .Property(x => x.OpenTime)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString() : null,
                    v => v != null ? TimeSpan.Parse(v) : (TimeSpan?)null
                );

            modelBuilder.Entity<GymBranch>()
                .Property(x => x.CloseTime)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString() : null,
                    v => v != null ? TimeSpan.Parse(v) : (TimeSpan?)null
                );


            // =============================
            // ✅ RELATIONSHIPS
            // =============================

            // ----- GymOwner -----
            modelBuilder.Entity<GymOwner>()
                .HasOne(o => o.Wallet)
                .WithOne(w => w.Owner)
                .HasForeignKey<OwnerWallet>(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GymOwner>()
                .HasMany(o => o.Branches)
                .WithOne(b => b.Owner)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GymOwner>()
                .HasMany(o => o.Settlements)
                .WithOne(s => s.Owner)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);


            // ----- GymBranch -----
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Staff)
                .WithOne(s => s.Branch)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Amenities)
                .WithOne(a => a.Branch)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Plans)
                .WithOne(p => p.Branch)
                .HasForeignKey(p => p.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Subscriptions)
                .WithOne(s => s.Branch)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Bookings)
                .WithOne(bk => bk.Branch)
                .HasForeignKey(bk => bk.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Visits)
                .WithOne(v => v.Branch)
                .HasForeignKey(v => v.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Reviews)
                .WithOne(r => r.Branch)
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- GymStaff -----
            modelBuilder.Entity<GymStaff>()
                .HasMany(s => s.VisitsCheckInHandled)
                .WithOne(v => v.Staff)
                .HasForeignKey(v => v.StaffId)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- GymPlan -----
            modelBuilder.Entity<GymPlan>()
                .HasMany(p => p.Subscriptions)
                .WithOne(s => s.Plan)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymPlan>()
                .HasMany(p => p.Bookings)
                .WithOne(b => b.Plan)
                .HasForeignKey(b => b.PlanId)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- User -----
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<UserWallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.WalletTransactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Visits)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- Subscription -----
            modelBuilder.Entity<Subscription>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Subscription)
                .HasForeignKey(b => b.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- Booking -----
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.VisitRecord)
                .WithOne(v => v.Booking)
                .HasForeignKey<Visit>(v => v.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Review)
                .WithOne(r => r.Booking)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);


            // ----- Visit -----
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.User)
                .WithMany(u => u.Visits)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Branch)
                .WithMany(b => b.Visits)
                .HasForeignKey(v => v.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Staff)
                .WithMany(s => s.VisitsCheckInHandled)
                .HasForeignKey(v => v.StaffId)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- Review -----
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Branch)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

