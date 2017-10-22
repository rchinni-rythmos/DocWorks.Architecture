using DocWorks.BuildingBlocks.ErrorHandling.Model;
using DocWorks.CMS.Api.Infrastructure.ActionResult;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace DocWorks.CMS.Api.Infrastructure.Filter
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse();

            response.Message = "An error occurred. Try it again.";
            var exceptionDetail = new ExceptionDetail();
            exceptionDetail.ExceptionMessage = context.Exception.Message;
            exceptionDetail.StackTrace = context.Exception.StackTrace;
            exceptionDetail.InnerExceptionMessage = context.Exception.InnerException != null ? context.Exception.InnerException.Message : string.Empty;
            exceptionDetail.InnerExceptionMessage = context.Exception.InnerException != null ? context.Exception.InnerException.StackTrace : string.Empty;

            string customeErrorMessageString = JsonConvert.SerializeObject(exceptionDetail);
            logger.LogError(new EventId(context.Exception.HResult),
                    context.Exception, customeErrorMessageString
                    );
            if (env.IsDevelopment())
            {
                response.ExceptionDetail = exceptionDetail;
            }
            context.Result = new InternalServerErrorObjectResult(response);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //}
            context.ExceptionHandled = true;
        }
    }
}
