namespace SharedKernel.Maybe
{
    /// <summary>
    /// Contains extension methods for the Maybe class.
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Binds to the result of an asynchronous function and returns it.
        /// </summary>
        public static async Task<Maybe<TOut>> Bind<TIn, TOut>(this Maybe<TIn> maybe, Func<TIn, Task<Maybe<TOut>>> func) =>
            maybe.HasValue ? await func(maybe.Value) : Maybe<TOut>.None;

        /// <summary>
        /// Binds to the result of an asynchronous function and returns it, when the Maybe is wrapped in a Task.
        /// </summary>
        public static async Task<Maybe<TOut>> Bind<TIn, TOut>(this Task<Maybe<TIn>> maybeTask, Func<TIn, Task<Maybe<TOut>>> func)
        {
            var maybe = await maybeTask;
            return await maybe.Bind(func);
        }

        /// <summary>
        /// Binds to the result of a synchronous function and returns it.
        /// </summary>
        public static Maybe<TOut> Bind<TIn, TOut>(this Maybe<TIn> maybe, Func<TIn, Maybe<TOut>> func) =>
            maybe.HasValue ? func(maybe.Value) : Maybe<TOut>.None;

        /// <summary>
        /// Matches to the corresponding functions based on existence of the value for an asynchronous Maybe.
        /// </summary>
        public static async Task<TOut> Match<TIn, TOut>(
            this Task<Maybe<TIn>> maybeTask,
            Func<TIn, TOut> onSuccess,
            Func<TOut> onFailure)
        {
            var maybe = await maybeTask;
            return maybe.Match(onSuccess, onFailure);
        }

        /// <summary>
        /// Matches to the corresponding functions based on existence of the value for a synchronous Maybe.
        /// </summary>
        public static TOut Match<TIn, TOut>(
            this Maybe<TIn> maybe,
            Func<TIn, TOut> onSuccess,
            Func<TOut> onFailure) =>
            maybe.HasValue ? onSuccess(maybe.Value) : onFailure();
    }
}