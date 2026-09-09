using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.VisitManagement.ValueObjects;

public sealed class TrailerRegistration : ValueObject
{
    public string Value { get; }

    public TrailerRegistration(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Trailer registration is required.", nameof(value));
        }

        Value = new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new ArgumentException("Trailer registration is required.", nameof(value));
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
