namespace UrlShortener.Persistence.Contants
{
    internal static class TableNames
    {
        internal const string ShortenedUrls = nameof(ShortenedUrls);
        internal const string OutboxMessages = nameof(OutboxMessages);
        internal const string OutboxMessageConsumers = nameof(OutboxMessageConsumers);
        internal const string AuditTrails = nameof(AuditTrails);
    }
}