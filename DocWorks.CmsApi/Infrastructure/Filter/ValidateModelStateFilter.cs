using DocWorks.BuildingBlocks.ErrorHandling.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace DocWorks.CMS.Api.Infrastructure.Filter
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }
            var response = new ErrorResponse();
            response.Message = "Model validation error occured";
            response.ModelValidationErrors = context.ModelState.Select(m => new ModelErrorDetail { Field = m.Key, Message = context.ModelState[m.Key].Errors.Select(y => y.ErrorMessage).ToList<String>() }).ToList();
            context.Result = new BadRequestObjectResult(response);
        }
    }
}
