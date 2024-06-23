namespace SharedKernel.Messaging
{
    public interface IDispatcher
    {
        /// <summary>
        /// Asynchronously send a request to a single handler
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
        Task<TResponse> Send<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken = default) where TResponse : class
            ;

        /// <summary>
        /// Asynchronously send a request to a single handler
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
        Task<TResponse> Send<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default) where TResponse : class;

        //
        // 摘要:
        //     Asynchronously send a notification to multiple handlers
        //
        // 參數:
        //   notification:
        //     Notification object
        //
        //   cancellationToken:
        //     Optional cancellation token
        //
        // 傳回:
        //     A task that represents the publish operation.
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : IEvent;
    }
}