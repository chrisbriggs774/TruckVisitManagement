using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.VisitManagement.ValueObjects;

public sealed class VisitId : ValueObject
{
    public Guid Value { get; }

    private VisitId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Visit id cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static VisitId New() => new(Guid.NewGuid());

    public static VisitId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
