namespace MaintenanceAPI.Models
{
    public class Flat
    {
        public int FlatId { get; set; }
        public int BuildingId { get; set; }
        public string FlatNumber { get; set; }
        public int OwnerUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Building Building { get; set; }
        public User Owner { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; }
    }
}
