using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.VisitManagement.Entities;

public sealed class Collection : Entity<Guid>
{
    public string Reference { get; }
    public string Description { get; private set; }

    public Collection(Guid id, string reference, string description) : base(id)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            throw new ArgumentException("Collection reference is required.", nameof(reference));
        }

        Reference = reference.Trim().ToUpperInvariant();
        Description = description?.Trim() ?? string.Empty;
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
    }
}
