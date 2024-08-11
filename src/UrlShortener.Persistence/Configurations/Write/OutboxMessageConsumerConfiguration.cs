using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Persistence.Contants;
using UrlShortener.Persistence.Outbox;

namespace UrlShortener.Persistence.Configurations.Write
{
    internal sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
    {
        public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
        {
            builder.ToTable(TableNames.OutboxMessageConsumers);

            builder.HasKey(outboxMessageConsumer => new
            {
                outboxMessageConsumer.Id,
                outboxMessageConsumer.Name
            });
        }
    }
}