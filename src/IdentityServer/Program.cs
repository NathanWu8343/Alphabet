using IdentityServer.Configs;
using UrlShortener.Infrastructure;

namespace IdentityServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();

            builder.ConfigureSerilog();

            // Add services to the container.

            //builder.Services.AddControllers();
            //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            // var identityConfig = builder.Configuration.GetSection("Identity").Get<IdentityConfig>()!;

            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddInMemoryIdentityResources(IdentityConfig.GetResources())
            .AddInMemoryApiScopes(IdentityConfig.GetApiScopes())
            .AddInMemoryApiResources(IdentityConfig.GetApis())
            .AddInMemoryClients(IdentityConfig.GetClients())
            .AddDeveloperSigningCredential();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            //app.MapControllers();

            app.UseIdentityServer();
            app.Run();
        }
    }
}