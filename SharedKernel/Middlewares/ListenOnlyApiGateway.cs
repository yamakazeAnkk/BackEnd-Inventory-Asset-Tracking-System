using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Middlewares
{
    public class ListenOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // extract specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = (int)StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service Unavailable. Please check the API Gateway header.");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
