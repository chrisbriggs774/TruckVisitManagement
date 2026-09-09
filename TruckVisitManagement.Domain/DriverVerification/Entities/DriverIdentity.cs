using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.DriverVerification.Entities;

public sealed class DriverIdentity : Entity<Guid>
{
    public string IdentityDocumentNumber { get; }
    public string FullName { get; }

    public DriverIdentity(Guid id, string identityDocumentNumber, string fullName) : base(id)
    {
        if (string.IsNullOrWhiteSpace(identityDocumentNumber))
        {
            throw new ArgumentException("Identity document number is required.", nameof(identityDocumentNumber));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Driver full name is required.", nameof(fullName));
        }

        IdentityDocumentNumber = identityDocumentNumber.Trim().ToUpperInvariant();
        FullName = fullName.Trim();
    }
}
