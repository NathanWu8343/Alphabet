namespace SharedKernel.Core
{
    /// <summary>
    /// Represents the aggregate root.
    /// </summary>
    public abstract class AggregateRoot<TKey>
        : Entity<TKey>, IAggregateRoot
        where TKey : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class.
        /// </summary>
        /// <param name="id">The aggregate root identifier.</param>
        protected AggregateRoot(TKey id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class.
        /// </summary>
        /// <remarks>
        /// Required by EF Core.
        /// </remarks>
        protected AggregateRoot()
        {
        }

        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Gets the domain events. This collection is readonly.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

        /// <summary>
        /// Clears all the domain events from the <see cref="AggregateRoot{TKey}"/>.
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <summary>
        /// Adds the specified <see cref="IDomainEvent"/> to the <see cref="AggregateRoot{TKey}"/>.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    }
}