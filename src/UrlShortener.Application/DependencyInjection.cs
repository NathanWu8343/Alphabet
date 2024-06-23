﻿using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UrlShortener.Application.Extensions;

namespace UrlShortener.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the necessary services with the DI framework.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //services.AddMediatR(cfg =>
            //{
            //    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            //    cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            //    cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));

            //    //cfg.AddOpenBehavior(typeof(TransactionBehaviour<,>));
            //});

            services.AddMediator(cfg =>
            {
                //   cfg.AddConsumer<CreateShortUrlCommandHandler>();
                //cfg.AddConsumers(Assembly.GetExecutingAssembly());
                //cfg.AddConsumers(AssemblyReference.Assembly);
                cfg.AddConsumersFromAssemblyContaining(UrlShortener.Application.AssemblyReference.Assembly);
            });

            return services;
        }
    }
}