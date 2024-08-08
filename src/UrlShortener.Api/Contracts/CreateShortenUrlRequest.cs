using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Api.Contracts
{
    /// <summary>
    /// 創建參數
    /// </summary>
    public sealed record CreateShortenUrlRequest
    {
        /// <summary>
        /// 網址
        /// </summary>
        ///
        [Required]
        public string Url { get; set; } = string.Empty;

        public TestEnum MyProperty { get; set; }
    }

    /// <summary>
    /// test enum
    /// </summary>
    public enum TestEnum : int
    {
        //[Description("Test133")]
        Test1 = 1,

        //[Description("Test233")]
        Test2,

        //[Description("Test333")]
        Test3
    }
}