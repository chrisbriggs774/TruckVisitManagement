using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.DriverVerification.Entities;

public sealed class VerificationSession : Entity<Guid>
{
    public DriverIdentity DriverIdentity { get; }
    public VerificationResult Result { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public VerificationSession(Guid id, DriverIdentity driverIdentity, VerificationResult result) : base(id)
    {
        DriverIdentity = driverIdentity;
        Result = result;
        StartedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        CompletedAtUtc = DateTimeOffset.UtcNow;
    }
}
