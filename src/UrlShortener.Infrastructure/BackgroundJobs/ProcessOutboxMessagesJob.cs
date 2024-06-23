//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Hosting;
//using SharedKernel.Core;
//using System.Text.Json;
//using UrlShortener.Persistence;
//using UrlShortener.Persistence.Outbox;

//namespace UrlShortener.Infrastructure.BackgroundJobs
//{
//    internal class ProcessOutboxMessagesJob : BackgroundService
//    {
//        private readonly ApplicationDbContext _dbContext;
//        private readonly IPublisher _publisher;

//        public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IPublisher publisher)
//        {
//            _dbContext = dbContext;
//            _publisher = publisher;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            List<OutboxMessage> messages = await _dbContext
//           .Set<OutboxMessage>()
//            .Where(m => m.ProcessedAtUtc == null)
//           .Take(20)
//           .ToListAsync(stoppingToken);

//            foreach (OutboxMessage outboxMessage in messages)
//            {
//                IDomainEvent? domainEvent = JsonSerializer.Deserialize<IDomainEvent>(outboxMessage.Content);

//                if (domainEvent is null)
//                {
//                    continue;
//                }

//                await _publisher.Publish(domainEvent, stoppingToken);

//                outboxMessage.ProcessedAtUtc = DateTime.UtcNow;
//            }

//            await _dbContext.SaveChangesAsync();
//        }
//    }
//}