namespace TruckVisitManagement.Application.Commands.Abstractions;

/// <summary>
/// Marker for a command that produces a result of <typeparamref name="TResult"/>.
/// </summary>
public interface ICommand<TResult>
{
}

/// <summary>
/// Handles a command of type <typeparamref name="TCommand"/> and returns <typeparamref name="TResult"/>.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
