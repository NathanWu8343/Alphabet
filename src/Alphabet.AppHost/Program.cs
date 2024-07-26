using Alphabet.AppHost.Extensions;
using Alphabet.AppHost.Resources;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//var serviceBus = builder.AddConnectionString("Messaging");

//var cmder = builder.AddRedis("te")
//    .WithRedisCommander();

// infra
var redis = builder.AddRedisLocalContainer("redis", 6379).ExcludeFromManifest();
var rabbitmq = builder.AddExternalContainer("rabbitmq", "rabbitmq", "http", 15672).ExcludeFromManifest();
var seqlog = builder.AddSeq("seqlog").ExcludeFromManifest();

//var resource = builder.AddExternalHost(name: "hub-service", new Uri("https://google.com/"));
//var a = resource.GetEndpoint("https");
// ap
var ids4 = builder.AddProject<Projects.IdentityServer>("ids4")
    .WithReference(seqlog);

var api = builder
    .AddProject<Projects.UrlShortener_Api>("urlshortener-api")
    .WithReference(ids4)
    .WithReference(seqlog);

var ingress = builder
   .AddProject<Projects.ApiGateway>("ingress")
   .WithReference(ids4)
   .WithReference(api)
   .WithReference(seqlog);

//api.WithEnvironment("INGRESS_ENDPOINT", ingress.GetEndpoint("https"));

builder.Build().Run();