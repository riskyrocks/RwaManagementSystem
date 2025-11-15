namespace MaintenanceAPI.Models
{
    public class Building
    {
        public int BuildingId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int FlatsCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Flat> Flats { get; set; }
    }
}
