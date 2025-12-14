using AutoMapper;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces; // تأكد إن ده الـ Namespace الصح للـ Interface
using FitHubBackendAPI.Services.Interfaces.UserServices; // لو الـ Interface هنا
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IGenericRepository<Subscription> _subRepo;
        private readonly IGenericRepository<GymPlan> _planRepo;
        private readonly IGenericRepository<GymBranch> _branchRepo; // ✅ 1. ضفنا ريبو الفروع هنا
        private readonly IGenericRepository<UserWallet> _walletRepo;
        private readonly IGenericRepository<UserCreditTransactions> _transRepo;
        private readonly IMapper _mapper;
        
        public SubscriptionService(
            IGenericRepository<Subscription> subRepo,
            IGenericRepository<GymPlan> planRepo,
            IGenericRepository<GymBranch> branchRepo, // ✅ 2. ضفناه في الـ Constructor
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transRepo,
            IMapper mapper)
        {
            _subRepo = subRepo;
            _planRepo = planRepo;
            _branchRepo = branchRepo; // ✅ 3. عملنا Assign
            _walletRepo = walletRepo;
            _transRepo = transRepo;
            _mapper = mapper;
        }

        // ====================================================
        // 1. شراء اشتراك (Purchase)
        // ====================================================

        public async Task<ResponseViewModel<bool>> PurchaseSubscriptionAsync(int userId, PurchaseSubscriptionDto dto)
        {
            try
            {
                // 1. هات تفاصيل الخطة اللي اليوزر عايزها
                var plan = await _planRepo.GetByIdAsync(dto.PlanId);
                if (plan == null || plan.Status != PlanStatus.ACTIVE)
                    return ResponseViewModel<bool>.Fail("Plan not found or inactive");

                // تأكد إن الخطة ليها سعر بالكريديت
                if (plan.CreditsCost == null)
                    return ResponseViewModel<bool>.Fail("This plan cannot be purchased with credits");

                // 2. هات محفظة اليوزر
                var wallets = await _walletRepo.FindAsync(w => w.UserId == userId);
                var wallet = wallets.FirstOrDefault();

                if (wallet == null || wallet.Balance < plan.CreditsCost)
                    return ResponseViewModel<bool>.Fail("Insufficient balance");

                // 3. خصم الرصيد
                int cost = plan.CreditsCost.Value;
                int oldBalance = wallet.Balance;

                wallet.Balance -= cost;
                wallet.LastUpdated = DateTime.UtcNow;
                _walletRepo.Update(wallet);

                // 4. تسجيل حركة الخصم (Transaction)
                var transaction = new UserCreditTransactions
                {
                    UserId = userId,
                    CreditsBefore = oldBalance,
                    CreditsChanged = -cost, // بالسالب عشان خصم
                    CreditsAfter = wallet.Balance,
                    TransactionType = TransactionType.DEDUCT, // أو PURCHASE حسب الـ Enum بتاعك
                    Source = TransactionSource.SUBSCRIPTION,  // مصدرها اشتراك
                    IsAcTive = true
                };
                await _transRepo.AddAsync(transaction);

                // 5. إنشاء الاشتراك الفعلي
                var subscription = new Subscription
                {
                    UserId = userId,
                    PlanId = plan.Id,
                    BranchId = plan.BranchId, // الفرع جبناه من الخطة

                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(plan.DurationDays ?? 30), // لو مفيش مدة، الديفولت 30 يوم

                    VisitsAllowed = plan.VisitsLimit ?? 0, // عدد الزيارات المسموحة
                    VisitsUsed = 0,

                    Status = SubscriptionStatus.ACTIVE,
                    IsAcTive = true
                };

                await _subRepo.AddAsync(subscription);

                // 6. حفظ الكل
                await _subRepo.SaveChangesAsync(); // Generic Repo بيسيف الكونتكست كله

                return ResponseViewModel<bool>.Success(true, "Subscription purchased successfully");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<bool>.Fail($"Error purchasing subscription: {ex.Message}");
            }
        }

        // ====================================================
        // 2. إلغاء اشتراك (Cancel)
        // ====================================================
        public async Task<ResponseViewModel<bool>> CancelSubscriptionAsync(int userId, int subscriptionId)
        {
            try
            {
                var sub = await _subRepo.GetByIdAsync(subscriptionId);

                // لازم نتأكد إن الاشتراك موجود وبتاع اليوزر ده
                if (sub == null || sub.UserId != userId)
                    return ResponseViewModel<bool>.Fail("Subscription not found");

                if (sub.Status == SubscriptionStatus.CANCELLED || sub.Status == SubscriptionStatus.EXPIRED)
                    return ResponseViewModel<bool>.Fail("Subscription is already cancelled or expired");

                // تغيير الحالة
                sub.Status = SubscriptionStatus.CANCELLED;

                // تصفير الزيارات المتبقية (عشان نضمن عدم الاستخدام)
                // نخلي المستخدم = المسموح، فالمتبقى يبقى صفر
                sub.VisitsUsed = sub.VisitsAllowed;
                sub.UpdatedAt = DateTime.UtcNow;

                _subRepo.Update(sub);
                await _subRepo.SaveChangesAsync();

                return ResponseViewModel<bool>.Success(true, "Subscription cancelled successfully");
            }
            catch (Exception ex)
            {
                return ResponseViewModel<bool>.Fail($"Error cancelling subscription: {ex.Message}");
            }
        }

        // ====================================================
        // 3. عرض اشتراكاتي (Get My Subscriptions)
        // ====================================================
        public async Task<ResponseViewModel<List<MySubscriptionDto>>> GetMySubscriptionsAsync(int userId)
        {
            // هنا محتاجين Includes عشان نجيب اسم الفرع واسم الخطة
            // الـ Generic Repo بتاعك فيه FindAsync بياخد predicate بس، 
            // لو مبيدعمش Include لازم نعدله، أو نستخدم حل مؤقت:
            // الحل: هنجيب الاشتراكات وبعدين نملا البيانات الناقصة (مش أحسن أداء بس شغال حالياً)
            // *الأفضل:* تضيف دالة في الريبو بتقبل Includes، بس خلينا نمشي بالمتاح دلوقت.

            var subs = await _subRepo.FindAsync(s => s.UserId == userId);

            // عشان نجيب أسماء الخطط والفروع، ممكن نعمل Loop أو نعدل الريبو.
            // هنا هفترض إننا هنستخدم AutoMapper وهو شاطر وممكن يظبطها لو عملنا Load للداتا.
            // **تعديل مهم:** عشان الداتا ترجع كاملة، لازم الـ Context يكون محمل الـ Navigation Properties.
            // بما إننا شغالين Generic بسيط، هنضطر نعمل Mapping يدوي أو نعدل الريبو لاحقاً.

            // (توضيح: الكود ده هيجيب الـ IDs بس، أسماء الفروع مش هتظهر إلا لو الـ Lazy Loading شغال أو عملنا Include).
            // عشان منتعطلش، هكتبلك كود بيجيب تفاصيل الخطة والفرع لكل اشتراك يدوي (For Loop) وده شغال 100%.

            var resultList = new List<MySubscriptionDto>();

            foreach (var sub in subs)
            {
                // بنجيب الخطة عشان الاسم
                var plan = await _planRepo.GetByIdAsync(sub.PlanId);

                // ✅ التعديل الجديد: بنجيب الفرع عشان ناخد اسمه الحقيقي
                var branch = await _branchRepo.GetByIdAsync(sub.BranchId);

                // الحل الأسرع: مابينج بسيط هنا
                resultList.Add(new MySubscriptionDto
                {
                    Id = sub.Id,
                    PlanName = plan?.Name ?? "Unknown Plan",

                    // ✅ هنا حطينا اسم الجيم الحقيقي بدل "Loading..."
                    // لو الـ branch رجع null بنحط "Unknown Gym" احتياطي
                    GymName = branch?.BranchName ?? "Unknown Gym",

                    StartDate = sub.StartDate,
                    EndDate = sub.EndDate,
                    VisitsAllowed = sub.VisitsAllowed,
                    VisitsUsed = sub.VisitsUsed,
                    Status = sub.Status
                });
            }

            // ملحوظة: عشان نجيب اسم الجيم صح، حقنا IGenericRepository<GymBranch> وعملنا GetById
            // بس كبداية الكود ده هيشغلك.

            return ResponseViewModel<List<MySubscriptionDto>>.Success(resultList);
        }
    }
}