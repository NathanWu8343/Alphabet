namespace UrlShortener.Application.Abstractions
{
    /// <summary>
    /// This is a shared context between Mediat Behaviors and RequestHandlers.
    /// </summary>
    public interface ISharedContext
    {
        /// <summary>
        /// Get Value From Shared Context.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Set value in Shared Context.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void Set(string key, object value);
    }

    public class SharedContext : Dictionary<string, object>, ISharedContext
    {
        public object Get(string key)
        {
            TryGetValue(key, out var value);
            return value;
        }

        public void Set(string key, object value)
            => Add(key, value);
    }
}