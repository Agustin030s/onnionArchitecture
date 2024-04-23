namespace Domain.Common
{
    public abstract class AuditableBaseEntity
    {
        public virtual int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
