using System;

namespace NirmalaAstrology_Web.Models
{
    public static class AppointmentStatuses
    {
        public const string PendingPayment = "PendingPayment";
        public const string PendingApproval = "PendingApproval";
        public const string Approved = "Approved";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Rejected = "Rejected";

        public static readonly string[] ActiveSlotStatuses =
        {
            PendingPayment, PendingApproval, Approved
        };

        public static bool IsKnown(string status)
        {
            return string.Equals(status, PendingPayment, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, PendingApproval, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, Approved, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, Completed, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, Cancelled, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, Rejected, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class PaymentStatuses
    {
        public const string Pending = "Pending";
        public const string AwaitingVerification = "AwaitingVerification";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";
    }

    public sealed class AppServiceCatalogItem
    {
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal PriceInr { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class AppointmentRecord
    {
        public int AppointmentId { get; set; }
        public string BookingRef { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public string AppointmentStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string Notes { get; set; }
        public string AdminNotes { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public sealed class PaymentRecord
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int? AppointmentId { get; set; }
        public int? ReportOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Gateway { get; set; }
        public string OrderId { get; set; }
        public string TransactionRef { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string GatewayResponseRef { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
