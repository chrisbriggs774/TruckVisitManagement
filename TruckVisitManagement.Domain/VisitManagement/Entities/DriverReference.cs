using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.VisitManagement.Entities;

public sealed class DriverReference : Entity<Guid>
{
    public string ExternalDriverId { get; private set; }

    public DriverReference(Guid id, string externalDriverId) : base(id)
    {
        if (string.IsNullOrWhiteSpace(externalDriverId))
        {
            throw new ArgumentException("Driver reference id is required.", nameof(externalDriverId));
        }

        ExternalDriverId = externalDriverId.Trim();
    }
}
