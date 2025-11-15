namespace MaintenanceAPI.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public int FlatId { get; set; }
        public string PlanType { get; set; } // Basic / Premium
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int VisitsPlumbing { get; set; }
        public int VisitsElectrical { get; set; }
        public DateTime CreatedAt { get; set; }

        public Flat Flat { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
