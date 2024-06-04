using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.UrlShortener
{
    public sealed record CreateShortenUrlRequest
    {
        /// <summary>
        /// 網址
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}