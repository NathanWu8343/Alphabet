using MassTransit;

namespace SharedKernel.Messaging
{
    /// <summary>
    /// Represents the event handler interface.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    public interface IEventHandler<in TEvent>
        where TEvent : class, IEvent
    {
    }
}