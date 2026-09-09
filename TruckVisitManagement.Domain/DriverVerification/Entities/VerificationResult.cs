using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.DriverVerification.Enums;

namespace TruckVisitManagement.Domain.DriverVerification.Entities;

public sealed class VerificationResult : Entity<Guid>
{
    public VerificationOutcome Outcome { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset EvaluatedAtUtc { get; private set; }

    public VerificationResult(Guid id) : base(id)
    {
        Outcome = VerificationOutcome.Pending;
        EvaluatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkVerified()
    {
        Outcome = VerificationOutcome.Verified;
        Reason = null;
        EvaluatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkRejected(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Rejection reason is required.", nameof(reason));
        }

        Outcome = VerificationOutcome.Rejected;
        Reason = reason.Trim();
        EvaluatedAtUtc = DateTimeOffset.UtcNow;
    }
}
