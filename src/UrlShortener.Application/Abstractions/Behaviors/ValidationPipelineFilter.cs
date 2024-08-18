using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace UrlShortener.Application.Abstractions.Behaviors
{
    /// <summary>
    /// ValidationPipelineFilter 是一個用於 MassTransit 的過濾器，
    /// 它在處理請求之前執行驗證邏輯，並在驗證失敗時回傳相應的錯誤回應。
    /// </summary>
    /// <typeparam name="TRequest">要驗證的請求類型。</typeparam>
    internal sealed class ValidationPipelineFilter<TRequest> : IFilter<ConsumeContext<TRequest>>
        where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationPipelineFilter<TRequest>> _logger;

        /// <summary>
        /// 建構函式，注入日誌紀錄器和驗證器集合。
        /// </summary>
        /// <param name="logger">日誌紀錄器。</param>
        /// <param name="validators">針對 TRequest 的驗證器集合。</param>
        public ValidationPipelineFilter(ILogger<ValidationPipelineFilter<TRequest>> logger, IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
            _logger = logger;
        }

        /// <summary>
        /// 設定探測點，用於記錄此過濾器的運作情況以便於診斷與分析。
        /// </summary>
        /// <param name="context">ProbeContext 用於記錄訊息。</param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope(nameof(ValidationPipelineFilter<TRequest>));
        }

        /// <summary>
        /// 處理請求前執行驗證，若驗證失敗則回傳錯誤結果。
        /// </summary>
        /// <param name="context">ConsumeContext 包含請求的上下文訊息。</param>
        /// <param name="next">IPipe 用於傳遞管道的後續處理。</param>
        public async Task Send(ConsumeContext<TRequest> context, IPipe<ConsumeContext<TRequest>> next)
        {
            // 執行驗證並取得所有的驗證錯誤
            var validationFailures = await ValidateAsync(context.Message);

            // 如果存在驗證錯誤
            if (validationFailures.Any())
            {
                // 記錄驗證錯誤的日誌
                LogValidationFailure(context, validationFailures);

                // 取得請求中的回應類型
                var responseType = GetRequestResponseType(context.Message);

                if (responseType is not null)
                {
                    // 建立自定義的驗證錯誤物件
                    var validationError = CreateValidationError(validationFailures);

                    // 根據回應類型回傳相應的錯誤結果
                    await RespondWithValidationError(context, responseType, validationError);
                    return;
                }

                // 如果無法生成合適的回應，則拋出驗證例外
                throw new ValidationException(validationFailures);
            }

            // 如果驗證通過，繼續執行後續的處理
            await next.Send(context);
        }

        /// <summary>
        /// 執行請求的驗證，並收集所有的驗證錯誤。
        /// </summary>
        /// <param name="request">待驗證的請求物件。</param>
        /// <returns>包含驗證錯誤的陣列。</returns>
        private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
        {
            // 如果沒有可用的驗證器，直接返回空的錯誤陣列
            if (!_validators.Any()) return Array.Empty<ValidationFailure>();

            // 創建驗證上下文
            var context = new ValidationContext<TRequest>(request);

            // 同步執行所有驗證器並收集驗證結果
            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context)));

            // 將所有驗證錯誤合併成一個陣列
            return validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToArray();
        }

        /// <summary>
        /// 建立 ValidationError 物件，包含所有的錯誤訊息。
        /// </summary>
        /// <param name="validationFailures">驗證錯誤的集合。</param>
        /// <returns>自定義的驗證錯誤物件。</returns>
        private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
            new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());

        /// <summary>
        /// 取得請求的泛型回應類型，用於決定如何生成回應結果。
        /// </summary>
        /// <param name="request">請求物件。</param>
        /// <returns>請求的泛型回應類型。</returns>
        private static Type? GetRequestResponseType(object request)
        {
            var requestType = request.GetType();
            var responseType = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Request<>));

            return responseType?.GetGenericArguments().FirstOrDefault();
        }

        /// <summary>
        /// 記錄驗證失敗的日誌訊息。
        /// </summary>
        /// <param name="context">請求上下文訊息。</param>
        /// <param name="validationFailures">驗證錯誤的集合。</param>
        private void LogValidationFailure(ConsumeContext<TRequest> context, ValidationFailure[] validationFailures)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Request: {RequestName} failed with error: {Error}.",
                requestName, validationFailures);
        }

        /// <summary>
        /// 根據回應類型回傳驗證錯誤的結果。
        /// </summary>
        /// <param name="context">請求上下文訊息。</param>
        /// <param name="responseType">請求中的回應類型。</param>
        /// <param name="validationError">驗證錯誤物件。</param>
        private async Task RespondWithValidationError(ConsumeContext<TRequest> context, Type responseType, ValidationError validationError)
        {
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = responseType.GetGenericArguments()[0];
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(resultType)
                    .GetMethod(nameof(Result<object>.ValidationFailure));

                if (failureMethod is not null)
                {
                    // 使用反射調用 ValidationFailure 方法生成結果
                    var result = failureMethod.Invoke(null, new object[] { validationError });
                    await context.RespondAsync(result);
                    return;
                }
            }
            else if (responseType == typeof(Result))
            {
                // 如果回應類型為非泛型 Result，直接生成失敗結果
                var result = Result.Failure(validationError);
                await context.RespondAsync(result);
            }
        }
    }
}