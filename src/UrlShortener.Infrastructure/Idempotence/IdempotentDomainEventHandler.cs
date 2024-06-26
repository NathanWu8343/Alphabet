﻿//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using SharedKernel.Core;
//using UrlShortener.Persistence;
//using UrlShortener.Persistence.Outbox;

//namespace UrlShortener.Infrastructure.Idempotence
//{
//    internal sealed class IdempotentDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
//        where TDomainEvent : IDomainEvent
//    {
//        private readonly INotificationHandler<TDomainEvent> _decorated;
//        private readonly ApplicationDbContext _dbContext;

//        public IdempotentDomainEventHandler(
//            INotificationHandler<TDomainEvent> decorated,
//            ApplicationDbContext dbContext)
//        {
//            _decorated = decorated;
//            _dbContext = dbContext;
//        }

//        public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
//        {
//            string consumer = _decorated.GetType().Name;

//            if (await _dbContext.Set<OutboxMessageConsumer>()
//                    .AnyAsync(
//                        outboxMessageConsumer =>
//                            outboxMessageConsumer.Id == notification.Id &&
//                            outboxMessageConsumer.Name == consumer,
//                        cancellationToken))
//            {
//                return;
//            }

//            await _decorated.Handle(notification, cancellationToken);

//            _dbContext.Set<OutboxMessageConsumer>()
//                .Add(new OutboxMessageConsumer
//                {
//                    Id = notification.Id,
//                    Name = consumer
//                });

//            await _dbContext.SaveChangesAsync(cancellationToken);
//        }
//    }
//}