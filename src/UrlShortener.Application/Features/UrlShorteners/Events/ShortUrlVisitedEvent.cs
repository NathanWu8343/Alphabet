using SharedKernel.Messaging;

namespace UrlShortener.Application.Features.UrlShorteners.Events
{
    public sealed record class ShortUrlVisitedEvent(string Code) : IEvent
    {
    }
}