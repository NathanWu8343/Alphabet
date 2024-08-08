using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SharedKernel.Errors;
using SharedKernel.Maybe;
using SharedKernel.Results;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using UrlShortener.Api.Abstractions;
using UrlShortener.Api.Contracts;
using UrlShortener.Api.Misc;
using UrlShortener.Application.Features.UrlShorteners.Commands;
using UrlShortener.Application.Features.UrlShorteners.Queries;
using OpenTelemetry.Trace;

namespace UrlShortener.Api.Controllers.v1
{
    /// <summary>
    /// 短網址產生器
    /// </summary>
    ///
    [ApiVersion(1)]
    public sealed class ShortenController : ApiController
    {
        private readonly ILogger<ShortenController> _logger;

        private readonly ActivitySource _activitySource;
        private readonly Counter<long> _freezingDaysCounter;

        public ShortenController(ILogger<ShortenController> logger, Instrumentation instrumentation)
        {
            _logger = logger;

            ArgumentNullException.ThrowIfNull(instrumentation);
            this._activitySource = instrumentation.ActivitySource;
            this._freezingDaysCounter = instrumentation.FreezingDaysCounter;
        }

        /// <summary>
        /// 創建短網址
        /// </summary>
        /// <param name="proxyPath"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// ```json
        /// {
        ///  "url": "http://google.com"
        /// }
        /// ```
        /// </remarks>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromHeader(Name = "x-proxy-api")] string? proxyPath, [FromQuery] TestEnum testEnum, [FromBody] CreateShortenUrlRequest request)
        {
            // Optional: Manually create an activity. This will become a child of
            // the activity created from the instrumentation library for AspNetCore.
            // Manually created activities are useful when there is a desire to track
            // a specific subset of the request. In this example one could imagine
            // that calculating the forecast is an expensive operation and therefore
            // something to be distinguished from the overall request.
            // Note: Tags can be added to the current activity without the need for
            // a manual activity using Activity.Current?.SetTag()
            using var activity = _activitySource.StartActivity("calculate forecast");

            _logger.LogInformation("hello1");

            Log.Information("hello2");

            // Optional: Count the freezing days
            _freezingDaysCounter.Add(1);

            var path = string.IsNullOrEmpty(proxyPath)
                ? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api"
                : $"{HttpContext.Request.Headers["Origin"]}/{proxyPath}";

            return await Result.Create(request, new Error("test", "demo", ErrorType.Validation))
                .Map(req => new CreateShortUrlCommand(req.Url, path))
                .Bind(cmd => Dispatcher.Send(cmd, CancellationToken))
                .Match(Ok, Failure);
        }

        /// <summary>
        /// 轉址
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/{code}")]
        public async Task<IActionResult> Visit([Required] string code)
        {
            var result = await Maybe<GetVisitShortenUrlByCodeQuery>
                .From(new GetVisitShortenUrlByCodeQuery(code))
                .Bind(query => Dispatcher.Send(query, CancellationToken));

            return result.HasValue ? Redirect(result.Value) : NotFound();
        }

        /// <summary>
        /// 獲取 訪問數量
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("visit/{code}/count")]
        public async Task<IActionResult> GetVisitCount([Required] string code)
        {
            return await Maybe<GtUrlVistorCountByCodeQuery>
                    .From(new GtUrlVistorCountByCodeQuery(code))
                    .Bind(query => Dispatcher.Send(query, CancellationToken))
                    .Match(Ok, NotFound);
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10)
        {
            return await Maybe<GetShortenUrlListQuery>
                    .From(new GetShortenUrlListQuery(page, pageSize))
                    .Bind(query => Dispatcher.Send(query, CancellationToken))
                    .Match(Ok, NotFound);
        }
    }
}