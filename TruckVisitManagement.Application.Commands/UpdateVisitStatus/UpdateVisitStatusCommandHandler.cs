using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Commands.UpdateVisitStatus;

public sealed class UpdateVisitStatusCommandHandler : ICommandHandler<UpdateVisitStatusCommand, Visit>
{
    private readonly IVisitWriteStore _writeStore;

    public UpdateVisitStatusCommandHandler(IVisitWriteStore writeStore)
    {
        _writeStore = writeStore ?? throw new ArgumentNullException(nameof(writeStore));
    }

    public async Task<Visit> HandleAsync(UpdateVisitStatusCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var visit = await _writeStore.GetByIdAsync(VisitId.From(command.VisitId), cancellationToken);
        if (visit is null)
        {
            throw new VisitNotFoundException(command.VisitId);
        }

        ApplyTransition(visit, command.TargetStatus, command.OriginatingActor);

        await _writeStore.UpdateAsync(visit, cancellationToken);

        return visit;
    }

    private static void ApplyTransition(Visit visit, VisitStatus targetStatus, string originatingActor)
    {
        switch (targetStatus)
        {
            case VisitStatus.AtGate:
                visit.ArriveAtGate(originatingActor);
                break;
            case VisitStatus.OnSite:
                visit.EnterSite(originatingActor);
                break;
            case VisitStatus.Completed:
                visit.CheckOut(originatingActor);
                break;
            case VisitStatus.Cancelled:
                visit.Cancel(originatingActor);
                break;
            case VisitStatus.PreRegistered:
                throw new InvalidVisitStatusTransitionException(
                    "A visit cannot be transitioned back to Pre-Registered.");
            default:
                throw new InvalidVisitStatusTransitionException(
                    $"Unsupported target status '{targetStatus}'.");
        }
    }
}
