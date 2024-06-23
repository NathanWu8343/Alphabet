using MassTransit.Mediator;

namespace SharedKernel.Messaging
{
    /// <summary>
    /// Represents the command interface.
    /// </summary>
    /// <typeparam name="TResponse">The command response type.</typeparam>
    public interface ICommand<out TResponse> : Request<TResponse> where TResponse : class
    {
    }
}