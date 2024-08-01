using Alphabet.AppHost.Extensions;
using Alphabet.AppHost.Resources;

var builder = DistributedApplication.CreateBuilder(args);

//var serviceBus = builder.AddConnectionString("Messaging");

//var cmder = builder.AddRedis("te")
//    .WithRedisCommander();

// infra
var redis = builder.AddRedisLocalContainer("redis", 6379);
var rabbitmq = builder.AddExternalContainer("rabbitmq", "rabbitmq", "http", 15672);
var seqlog = builder.AddSeq(name: "seqlog").ExcludeFromManifest();
var grafana = builder.AddExternalContainer("grafana", "grafana", "http", 4317);
var grafanaui = builder.AddExternalContainer("grafana-ui", "grafana", "http", 3000);
//var grafana = builder.AddContainer("grafana", "grafana/otel-lgtm", "latest")
//    .WithEndpoint(3000, 3000) //UI
//    .WithEndpoint(4317, 4317) //grpc
//    .WithEndpoint(4318, 4318) //http
//    .WithContainerRuntimeArgs("--rm");

//var resource = builder.AddExternalHost(name: "hub-service", new Uri("https://google.com/"));
//var a = resource.GetEndpoint("https");
// ap
var ids4 = builder.AddProject<Projects.IdentityServer>("ids4")
    .WithReference(seqlog)
    .WithReference(grafana);

var api = builder
    .AddProject<Projects.UrlShortener_Api>("urlshortener-api")
    .WithReference(ids4)
    .WithReference(seqlog)
    .WithReference(grafana);

var ingress = builder
   .AddProject<Projects.ApiGateway>("ingress")
   .WithReference(ids4)
   .WithReference(api)
   .WithReference(seqlog)
   .WithReference(grafana);

//api.WithEnvironment("INGRESS_ENDPOINT", ingress.GetEndpoint("https"));

builder.Build().Run();