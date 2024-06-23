using SharedKernel.Messaging;

namespace SharedKernel.Core
{
    /// <summary>
    /// Represents the interface for an event that is raised within the domain.
    /// </summary>
    public interface IDomainEvent : IEvent
    {
        public Guid Id { get; init; }
    }
}