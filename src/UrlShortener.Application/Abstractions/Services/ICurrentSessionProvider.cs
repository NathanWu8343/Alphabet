namespace UrlShortener.Application.Abstractions.Services
{
    public interface ICurrentSessionProvider
    {
        Guid? GetUserId();
    }
}