using System.Text.RegularExpressions;

namespace UrlShortener.Api.Misc
{
    internal sealed class ToKebabParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object? value) => value != null
            ? Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower() // to kebab
            : null;
    }
}