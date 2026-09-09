using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.VisitManagement.Entities;

public sealed class Truck : Entity<Guid>
{
    public LicensePlate LicensePlate { get; private set; }
    public TruckUnitNumber UnitNumber { get; private set; }

    public Truck(Guid id, LicensePlate licensePlate, TruckUnitNumber unitNumber) : base(id)
    {
        LicensePlate = licensePlate;
        UnitNumber = unitNumber;
    }

    public void UpdateIdentity(LicensePlate licensePlate, TruckUnitNumber unitNumber)
    {
        LicensePlate = licensePlate;
        UnitNumber = unitNumber;
    }
}
