using SharedKernel.Core;

namespace UrlShortener.Domain.Entities
{
    public class VistorCounter : AggregateRoot<string>
    {
        public int Count { get; private set; }
    }
}