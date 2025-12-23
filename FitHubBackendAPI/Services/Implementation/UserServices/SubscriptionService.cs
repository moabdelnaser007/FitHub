using AutoMapper;
using FitHubBackendAPI.DTOs.Subscriptions;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Interfaces;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FitHubBackendAPI.ViewModels;

namespace FitHubBackendAPI.Services.Implementation.UserServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IGenericRepository<Subscription> _subRepo;
        private readonly IGenericRepository<GymPlan> _planRepo;
        private readonly IGenericRepository<GymBranch> _branchRepo;
        private readonly IGenericRepository<UserWallet> _walletRepo;
        private readonly IGenericRepository<UserCreditTransactions> _transRepo; // هنستخدم دي في هيستوري الاشتراك
        private readonly IMapper _mapper;

        public SubscriptionService(
            IGenericRepository<Subscription> subRepo,
            IGenericRepository<GymPlan> planRepo,
            IGenericRepository<GymBranch> branchRepo,
            IGenericRepository<UserWallet> walletRepo,
            IGenericRepository<UserCreditTransactions> transRepo,
            IMapper mapper)
        {
            _subRepo = subRepo;
            _planRepo = planRepo;
            _branchRepo = branchRepo;
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
                    return ResponseViewModel<bool>.Fail("Insufficient balance. Please recharge your wallet.");

                // 3. خصم الرصيد
                int cost = plan.CreditsCost.Value;
                int? oldBalance = wallet.Balance;

                wallet.Balance -= cost;
                wallet.LastUpdated = DateTime.UtcNow;
                // _walletRepo.Update(wallet); // EF Core tracks changes automatically

                // 4. تسجيل حركة الخصم (Transaction)
                var transaction = new UserCreditTransactions
                {
                    UserId = userId,
                    CreditsBefore = oldBalance.Value,
                    CreditsChanged = -cost, // بالسالب عشان خصم
                    CreditsAfter = wallet.Balance,

                    // هنا بنسجل إن المصدر هو اشتراك عشان تظهر في الهيستوري صح
                    TransactionType = TransactionType.DEDUCT,
                    Source = TransactionSource.SUBSCRIPTION,

                    // مهم: بنسجل المبلغ اللي اتدفع (PaymentAmount) عشان يظهر في الفاتورة
                    // هنا المبلغ هو التكلفة بالنقاط، لو عندك سعر بالفلوس ممكن تحطه
                    PaymentAmount = cost,

                    IsAcTive = true
                };

                // هنضيفها للكونتكس بس لسه مش هنسيف دلوقتي
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

                // 6. حفظ الكل (Save Changes) مرة واحدة
                await _subRepo.SaveChangesAsync();

                // تحديث الـ ReferenceId للترانزاكشن برقم الاشتراك الجديد
                // دي خطوة إضافية شيك عشان نربط الخصم بالاشتراك
                transaction.ReferenceId = subscription.Id;
                _transRepo.Update(transaction);
                await _transRepo.SaveChangesAsync();

                return ResponseViewModel<bool>.Success(true, "Subscription purchased successfully");
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ResponseViewModel<bool>.Fail($"Error purchasing subscription: {msg}");
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
            try
            {
                // بنجيب الاشتراكات الخاصة باليوزر
                var subs = await _subRepo.FindAsync(s => s.UserId == userId);
                var resultList = new List<MySubscriptionDto>();

                // بنعمل Loop عشان نملى البيانات الناقصة (اسم الفرع والخطة) يدوي
                foreach (var sub in subs)
                {
                    var plan = await _planRepo.GetByIdAsync(sub.PlanId);
                    var branch = await _branchRepo.GetByIdAsync(sub.BranchId);

                    resultList.Add(new MySubscriptionDto
                    {
                        Id = sub.Id,
                        PlanName = plan?.Name ?? "Unknown Plan",
                        GymName = branch?.BranchName ?? "Unknown Gym",
                        StartDate = sub.StartDate,
                        EndDate = sub.EndDate,
                        VisitsAllowed = sub.VisitsAllowed,
                        VisitsUsed = sub.VisitsUsed,
                        Status = sub.Status
                    });
                }

                return ResponseViewModel<List<MySubscriptionDto>>.Success(resultList);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<List<MySubscriptionDto>>.Fail($"Error fetching subscriptions: {ex.Message}");
            }
        }

        // ====================================================
        // 4. تفاصيل الاشتراك + سجل المدفوعات (Get Details) ✅ NEW
        // ====================================================
        public async Task<ResponseViewModel<SubscriptionDetailsDto>> GetSubscriptionDetailsAsync(int userId, int subscriptionId)
        {
            try
            {
                // 1️⃣ نجيب تفاصيل الاشتراك (الجزء اللي فوق)
                // بنحاول نجيب الـ Branch والـ Plan بالـ Include
                var subs = await _subRepo.GetAsync(
                    filter: s => s.Id == subscriptionId,
                    includeProperties: "Branch,Plan"
                );

                var subscription = subs.FirstOrDefault();

                // Validation Checks
                if (subscription == null)
                    return ResponseViewModel<SubscriptionDetailsDto>.Fail("Subscription not found");

                if (subscription.UserId != userId)
                    return ResponseViewModel<SubscriptionDetailsDto>.Fail("Unauthorized access to this subscription");

                // تحويل المودل لـ DTO (بيستخدم المابنج اللي عدلناه عشان Plan.Name)
                var dto = _mapper.Map<SubscriptionDetailsDto>(subscription);

                // 2️⃣ نجيب سجل المدفوعات (الجدول اللي تحت)
                // الشرط: لنفس اليوزر + المصدر اشتراك + الـ ReferenceId هو رقم الاشتراك ده
                var transactions = await _transRepo.GetAsync(
                    filter: t => t.UserId == userId &&
                                 t.Source == TransactionSource.SUBSCRIPTION &&
                                 t.ReferenceId == subscriptionId,
                    orderBy: q => q.OrderByDescending(t => t.CreatedAt)
                );

                // تحويل الترانزاكشنز لليستة جوه الـ DTO
                dto.BillingHistory = _mapper.Map<List<SubscriptionTransactionDto>>(transactions);

                return ResponseViewModel<SubscriptionDetailsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<SubscriptionDetailsDto>.Fail($"Error loading details: {ex.Message}");
            }
        }
    }
}