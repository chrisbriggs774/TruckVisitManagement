namespace TruckVisitManagement.Domain.Common;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; }

    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }
}
