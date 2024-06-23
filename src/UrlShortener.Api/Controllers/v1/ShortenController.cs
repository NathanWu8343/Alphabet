using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Errors;
using SharedKernel.Maybe;
using SharedKernel.Results;
using System.ComponentModel.DataAnnotations;
using UrlShortener.Api.Abstractions;
using UrlShortener.Api.Contracts;
using UrlShortener.Application.Features.UrlShorteners.Commands;
using UrlShortener.Application.Features.UrlShorteners.Queries;

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

        public ShortenController(ILogger<ShortenController> logger)
        {
            _logger = logger;
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
        public async Task<IActionResult> Create([FromHeader(Name = "x-proxy-api")] string? proxyPath, CreateShortenUrlRequest request)
        {
            var path = string.IsNullOrEmpty(proxyPath)
                ? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api"
                : $"{HttpContext.Request.Headers["Origin"]}/{proxyPath}";

            return await Result.Create(request, new Error("test", "demo", ErrorType.Validation))
                .Map(req => new CreateShortUrlCommand(req.Url, path))
                .Bind(cmd => Mediator.Send(cmd))
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
                .Bind(query => Mediator.Send(query));

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
            return await Maybe<GtUrlVistorCountQueryByCode>
                    .From(new GtUrlVistorCountQueryByCode(code))
                    .Bind(query => Mediator.Send(query))
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
                    .Bind(query => Mediator.Send(query))
                    .Match(Ok, NotFound);
        }
    }
}