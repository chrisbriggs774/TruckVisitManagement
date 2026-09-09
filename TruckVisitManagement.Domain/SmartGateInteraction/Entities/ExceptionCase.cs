using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.SmartGateInteraction.Entities;

public sealed class ExceptionCase : Entity<Guid>
{
    public string Code { get; }
    public string Description { get; }

    public ExceptionCase(Guid id, string code, string description) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Exception code is required.", nameof(code));
        }

        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim() ?? string.Empty;
    }
}
