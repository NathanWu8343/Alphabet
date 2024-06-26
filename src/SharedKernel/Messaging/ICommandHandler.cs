namespace SharedKernel.Messaging
{
    /// <summary>
    /// Represents the command handler interface.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResponse">The command response type.</typeparam>
    public interface ICommandHandler<in TCommand, out TResponse>
        where TCommand : class, ICommand<TResponse>
        where TResponse : class
    {
    }
}