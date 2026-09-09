using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.VisitManagement.Entities;

public sealed class Trailer : Entity<Guid>
{
    public TrailerNumber Number { get; private set; }
    public TrailerRegistration Registration { get; private set; }

    public Trailer(Guid id, TrailerNumber number, TrailerRegistration registration) : base(id)
    {
        Number = number;
        Registration = registration;
    }

    public void UpdateIdentity(TrailerNumber number, TrailerRegistration registration)
    {
        Number = number;
        Registration = registration;
    }
}
