using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.SmartGateInteraction.Enums;

namespace TruckVisitManagement.Domain.SmartGateInteraction.Entities;

public sealed class GateSession : Entity<Guid>
{
    private readonly List<ExceptionCase> _exceptions = new();

    public DriverSession DriverSession { get; }
    public InteractionWorkflow Workflow { get; }
    public SessionStatus Status { get; private set; }
    public IReadOnlyCollection<ExceptionCase> Exceptions => _exceptions.AsReadOnly();

    public GateSession(Guid id, DriverSession driverSession, InteractionWorkflow workflow) : base(id)
    {
        DriverSession = driverSession;
        Workflow = workflow;
        Status = SessionStatus.Active;
    }

    public void RaiseException(ExceptionCase exceptionCase)
    {
        _exceptions.Add(exceptionCase);
        Status = SessionStatus.Escalated;
    }

    public void Complete()
    {
        Status = SessionStatus.Completed;
    }
}
