namespace SharedKernel.Messaging
{
    /// <summary>
    /// Represents the query interface.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResponse">The query response type.</typeparam>
    public interface IQueryHandler<in TQuery, out TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
    }
}