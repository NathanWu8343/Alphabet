﻿using MediatR;

namespace SharedKernel.Core
{
    /// <summary>
    /// Represents the interface for an event that is raised within the domain.
    /// </summary>
    public interface IDomainEvent : INotification
    {
        public Guid Id { get; init; }
    }
}