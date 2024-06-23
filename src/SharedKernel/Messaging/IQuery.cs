using MassTransit.Mediator;

namespace SharedKernel.Messaging
{
    /// <summary>
    /// Represents the query interface.
    /// </summary>
    /// <typeparam name="TResponse">The query response type.</typeparam>
    public interface IQuery<out TResponse> : Request<TResponse> where TResponse : class
    {
    }
}