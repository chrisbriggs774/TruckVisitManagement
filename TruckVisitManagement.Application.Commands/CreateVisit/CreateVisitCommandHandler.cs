using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Commands.CreateVisit;

public sealed class CreateVisitCommandHandler : ICommandHandler<CreateVisitCommand, Visit>
{
    private readonly IVisitWriteStore _writeStore;

    public CreateVisitCommandHandler(IVisitWriteStore writeStore)
    {
        _writeStore = writeStore ?? throw new ArgumentNullException(nameof(writeStore));
    }

    public async Task<Visit> HandleAsync(CreateVisitCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Truck);

        var terminalId = new TerminalId(command.TerminalId);

        var truck = new Truck(
            Guid.NewGuid(),
            new LicensePlate(command.Truck.LicensePlate),
            new TruckUnitNumber(command.Truck.UnitNumber));

        var driverReference = new DriverReference(Guid.NewGuid(), command.ExternalDriverId);

        Trailer? trailer = null;
        if (command.Trailer is not null)
        {
            trailer = new Trailer(
                Guid.NewGuid(),
                new TrailerNumber(command.Trailer.Number),
                new TrailerRegistration(command.Trailer.Registration));
        }

        var collections = (command.Collections ?? Array.Empty<CreateVisitMovement>())
            .Select(c => new Collection(Guid.NewGuid(), c.Reference, c.Description))
            .ToList();

        var deliveries = (command.Deliveries ?? Array.Empty<CreateVisitMovement>())
            .Select(d => new Delivery(Guid.NewGuid(), d.Reference, d.Description))
            .ToList();

        var visit = new Visit(
            VisitId.New(),
            terminalId,
            truck,
            driverReference,
            collections,
            deliveries,
            trailer);

        await _writeStore.AddAsync(visit, cancellationToken);

        return visit;
    }
}
