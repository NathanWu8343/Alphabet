using Microsoft.AspNetCore.Http;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Infrastructure.Services
{
    public class CurrentSessionProvider : ICurrentSessionProvider
    {
        private readonly Guid? _currentUserId;

        public CurrentSessionProvider(IHttpContextAccessor accessor)
        {
            var userId = accessor.HttpContext?.User.FindFirst("userid")?.Value;

            if (userId is null)
            {
                return;
            }

            _currentUserId = Guid.TryParse(userId, out var guid) ? guid : null;
        }

        public Guid? GetUserId() => _currentUserId;
    }
}