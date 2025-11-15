namespace MaintenanceAPI.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; } = "Pending"; // Success / Failed / Pending
        public string PaymentGatewayId { get; set; }

        public Subscription Subscription { get; set; }
    }
}
