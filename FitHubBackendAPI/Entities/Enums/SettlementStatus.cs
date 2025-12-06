namespace FitHubBackendAPI.Entities.Enums
{
    public enum SettlementStatus
    {
        PENDING,        // لم يتم الدفع
        PAID,           // تم التحويل
        FAILED          // فشل عملية الدفع
    }
}
