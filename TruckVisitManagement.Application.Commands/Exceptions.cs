namespace TruckVisitManagement.Application.Commands;

/// <summary>
/// Thrown when a visit cannot be located in the write store.
/// </summary>
public sealed class VisitNotFoundException : Exception
{
    public Guid VisitId { get; }

    public VisitNotFoundException(Guid visitId)
        : base($"Visit '{visitId}' was not found.")
    {
        VisitId = visitId;
    }
}

/// <summary>
/// Thrown when a requested status transition is not valid.
/// </summary>
public sealed class InvalidVisitStatusTransitionException : Exception
{
    public InvalidVisitStatusTransitionException(string message) : base(message)
    {
    }
}
