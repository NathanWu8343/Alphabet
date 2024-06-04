using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Presentation.Abstractions
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected readonly ISender Sender;
        protected readonly ILogger Logger;

        protected ApiController(ILogger logger)
        {
            Logger = logger;
            Sender = this.HttpContext.RequestServices.GetRequiredService<ISender>();
        }
    }
}