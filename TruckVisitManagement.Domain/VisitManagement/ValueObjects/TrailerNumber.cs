using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.VisitManagement.ValueObjects;

public sealed class TrailerNumber : ValueObject
{
    public string Value { get; }

    public TrailerNumber(string value)
    {
        Value = Normalize(value, nameof(value), "Trailer number is required.");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static string Normalize(string value, string paramName, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(errorMessage, paramName);
        }

        var normalized = new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException(errorMessage, paramName);
        }

        return normalized;
    }
}
