using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.AuditCompliance.Entities;

public sealed class AuditTrail : Entity<Guid>
{
    private readonly List<AuditRecord> _records = new();

    public IReadOnlyCollection<AuditRecord> Records => _records.AsReadOnly();

    public AuditTrail(Guid id) : base(id)
    {
    }

    public void Append(AuditRecord record)
    {
        _records.Add(record);
    }
}
