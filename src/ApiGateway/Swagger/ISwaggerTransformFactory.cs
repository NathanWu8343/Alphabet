using Microsoft.OpenApi.Models;

namespace ApiGateway.Swagger
{
    public interface ISwaggerTransformFactory
    {
        bool Build(OpenApiOperation operation, IReadOnlyDictionary<string, string> transformValues);
    }
}