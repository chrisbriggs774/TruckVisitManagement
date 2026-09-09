using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.SmartGateInteraction.Entities;

public sealed class InteractionWorkflow : Entity<Guid>
{
    private readonly List<string> _steps = new();

    public string Name { get; }
    public IReadOnlyCollection<string> Steps => _steps.AsReadOnly();

    public InteractionWorkflow(Guid id, string name, IEnumerable<string> steps) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Workflow name is required.", nameof(name));
        }

        Name = name.Trim();
        _steps.AddRange(steps.Where(step => !string.IsNullOrWhiteSpace(step)).Select(step => step.Trim()));
    }
}
