using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Api.Contracts
{
    /// <summary>
    /// 創建參數
    /// </summary>
    public sealed record CreateShortenUrlRequest()
    {
        /// <summary>
        /// 網址
        /// </summary>
        ///
        [Required]
        public string Url { get; init; }
    }
}