namespace UCourses_Back_End.Core.Enums.CoreEnum
{
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Authorized = 2,
        Captured = 3,
        Failed = 4,
        Cancelled = 5,
        Expired = 6,
        Refunded = 7,
        PartiallyRefunded = 8,
        Voided = 9
    }
}