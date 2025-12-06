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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================
            // 🔵 PRIMARY KEYS
            // =====================================
            modelBuilder.Entity<GymOwner>().HasKey(x => x.OwnerId);
            modelBuilder.Entity<OwnerWallet>().HasKey(x => x.WalletId);
            modelBuilder.Entity<GymBranch>().HasKey(x => x.BranchId);
            modelBuilder.Entity<GymStaff>().HasKey(x => x.StaffId);
            modelBuilder.Entity<GymAmenities>().HasKey(x => x.AmenityId);
            modelBuilder.Entity<GymPlan>().HasKey(x => x.PlanId);
            modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<UserWallet>().HasKey(x => x.WalletId);
            modelBuilder.Entity<UserCreditTransactions>().HasKey(x => x.TransactionId);
            modelBuilder.Entity<Subscription>().HasKey(x => x.SubscriptionId);
            modelBuilder.Entity<Booking>().HasKey(x => x.BookingId);
            modelBuilder.Entity<Visit>().HasKey(x => x.VisitId);
            modelBuilder.Entity<Review>().HasKey(x => x.ReviewId);
            modelBuilder.Entity<OwnerSettlement>().HasKey(x => x.SettlementId);


            // =====================================
            // 🔵 ENUM CONVERSIONS
            // =====================================
            // GymOwner
            modelBuilder.Entity<GymOwner>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<GymOwner>().Property(x => x.ApplicationStatus).HasConversion<string>();

            // GymBranch
            modelBuilder.Entity<GymBranch>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<GymBranch>().Property(x => x.GenderType).HasConversion<string>();

            // GymStaff
            modelBuilder.Entity<GymStaff>().Property(x => x.Status).HasConversion<string>();

            // GymPlan
            modelBuilder.Entity<GymPlan>().Property(x => x.Status).HasConversion<string>();

            // User
            modelBuilder.Entity<User>().Property(x => x.Status).HasConversion<string>();

            // UserCreditTransactions
            modelBuilder.Entity<UserCreditTransactions>().Property(x => x.TransactionType).HasConversion<string>();
            modelBuilder.Entity<UserCreditTransactions>().Property(x => x.Source).HasConversion<string>();

            // Subscription
            modelBuilder.Entity<Subscription>().Property(x => x.Status).HasConversion<string>();

            // Booking
            modelBuilder.Entity<Booking>().Property(x => x.Status).HasConversion<string>();

            // Visit
            modelBuilder.Entity<Visit>().Property(x => x.Status).HasConversion<string>();

            // Settlement
            modelBuilder.Entity<OwnerSettlement>().Property(x => x.PayoutStatus).HasConversion<string>();


            // =====================================
            // 🔵 TIMESPAN CONVERSION
            // =====================================
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


            // =====================================
            // 🔵 RELATIONSHIPS
            // =====================================

            // GymOwner 1-1 OwnerWallet
            modelBuilder.Entity<GymOwner>()
                .HasOne(o => o.Wallet)
                .WithOne(w => w.Owner)
                .HasForeignKey<OwnerWallet>(w => w.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // GymOwner 1-M Branches
            modelBuilder.Entity<GymOwner>()
                .HasMany(o => o.Branches)
                .WithOne(b => b.Owner)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // GymOwner 1-M Settlements
            modelBuilder.Entity<GymOwner>()
                .HasMany(o => o.Settlements)
                .WithOne(s => s.Owner)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);


            // GymBranch 1-M Staff
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Staff)
                .WithOne(s => s.Branch)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            // GymBranch 1-M Amenities
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Amenities)
                .WithOne(a => a.Branch)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            // GymBranch 1-M Plans
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Plans)
                .WithOne(p => p.Branch)
                .HasForeignKey(p => p.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            // GymBranch 1-M Subscriptions
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Subscriptions)
                .WithOne(s => s.Branch)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // GymBranch 1-M Bookings
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Bookings)
                .WithOne(bk => bk.Branch)
                .HasForeignKey(bk => bk.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // GymBranch 1-M Visits
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Visits)
                .WithOne(v => v.Branch)
                .HasForeignKey(v => v.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // GymBranch 1-M Reviews
            modelBuilder.Entity<GymBranch>()
                .HasMany(b => b.Reviews)
                .WithOne(r => r.Branch)
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Restrict);


            // GymStaff 1-M Visit CheckIns
            modelBuilder.Entity<GymStaff>()
                .HasMany(s => s.VisitsCheckInHandled)
                .WithOne(v => v.Staff)
                .HasForeignKey(v => v.StaffId)
                .OnDelete(DeleteBehavior.Restrict);


            // GymPlan 1-M Subscriptions
            modelBuilder.Entity<GymPlan>()
                .HasMany(p => p.Subscriptions)
                .WithOne(s => s.Plan)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // GymPlan 1-M Bookings
            modelBuilder.Entity<GymPlan>()
                .HasMany(p => p.Bookings)
                .WithOne(b => b.Plan)
                .HasForeignKey(b => b.PlanId)
                .OnDelete(DeleteBehavior.Restrict);


            // User 1-1 Wallet
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<UserWallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 1-M Wallet Transactions
            modelBuilder.Entity<User>()
                .HasMany(u => u.WalletTransactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // User 1-M Subscriptions
            modelBuilder.Entity<User>()
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 1-M Bookings
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 1-M Visits
            modelBuilder.Entity<User>()
                .HasMany(u => u.Visits)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 1-M Reviews
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // Subscription 1-M Bookings
            modelBuilder.Entity<Subscription>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Subscription)
                .HasForeignKey(b => b.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);


            // Booking 1-1 Visit (Optional until Check-In happens)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.VisitRecord)
                .WithOne(v => v.Booking)
                .HasForeignKey<Visit>(v => v.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking 1-1 Review (Optional)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Review)
                .WithOne(r => r.Booking)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);


            // Visit Relations
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


            // Review Relations
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


            // Owner Settlements
            modelBuilder.Entity<GymOwner>()
                .HasMany(o => o.Settlements)
                .WithOne(s => s.Owner)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseEntity>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<BaseEntity>()
                .Property(e => e.UpdatedAt)
                .ValueGeneratedOnUpdate();


        }
    }
}
