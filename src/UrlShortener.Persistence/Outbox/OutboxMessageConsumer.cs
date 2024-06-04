using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Persistence.Outbox
{
    public sealed class OutboxMessageConsumer
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}