namespace FitHubBackendAPI.Entities.Enums
{
    public enum TransactionSource
    {
        BOOKING,         // مرتبط بحجز
        VISIT,           // مرتبط بزيارة
        SUBSCRIPTION,    // خصم من اشتراك زيارته
        MANUAL,          // إضافة/خصم يدوي
        SETTLEMENT,      // عملية تسوية
        PAYMOB           // عملية دفع عبر باي موب
    }
}
