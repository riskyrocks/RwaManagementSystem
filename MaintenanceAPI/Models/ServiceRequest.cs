namespace MaintenanceAPI.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public int FlatId { get; set; }
        public string ServiceType { get; set; } // Plumbing / Electrical
        public DateTime RequestedDate { get; set; }
        public int? AssignedTechId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending / In Progress / Completed
        public decimal Cost { get; set; } = 0; // Extra cost for emergency visits
        public DateTime CreatedAt { get; set; }

        public Flat Flat { get; set; }
        public User AssignedTechnician { get; set; }
    }
}
