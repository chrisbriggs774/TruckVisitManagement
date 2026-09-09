using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.VisitManagement.ValueObjects;

public sealed class TerminalId : ValueObject
{
    public string Value { get; }

    public TerminalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Terminal id is required.", nameof(value));
        }

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
