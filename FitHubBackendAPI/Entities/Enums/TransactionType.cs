namespace FitHubBackendAPI.Entities.Enums
{
    public enum TransactionType
    {
        RECHARGE,        // شحن كريديت
        DEDUCT,          // خصم كريديت للزيارة
        REFUND,          // استرداد كريديت
        ADMIN_ADJUST     // تعديل بواسطة الإدارة
    }
}
