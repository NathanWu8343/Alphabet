using Alphabet.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

//var serviceBus = builder.AddConnectionString("Messaging");

//var cmder = builder.AddRedis("te")
//    .WithRedisCommander();

// infra
var redis = builder.AddRedisLocalContainer("redis");

//var resource = builder.AddExternalHost(name: "hub-service", new Uri("https://google.com/"));
//var a = resource.GetEndpoint("https");
// ap
var ids4 = builder.AddProject<Projects.IdentityServer>("ids4");

var api = builder
    .AddProject<Projects.UrlShortener_Api>("urlshortener-api")
    .WithReference(ids4);

var ingress = builder
   .AddProject<Projects.ApiGateway>("ingress")
   .WithReference(ids4)
   .WithReference(api);
// .WithReference(a);

//api.WithEnvironment("INGRESS_ENDPOINT", ingress.GetEndpoint("https"));

builder.Build().Run();