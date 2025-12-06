namespace FitHubBackendAPI.Entities
{
    public class BaseEntity
    {
        public int ID { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsAcTive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
