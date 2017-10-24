using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Model.ErrorHandling
{
    public class ErrorResponse
    {
        public List<ModelValidationErrorDetail> ModelValidationErrors { get; set; }
        public string Message { get; set; }
        public ExceptionDetail ExceptionDetail { get; set; }
        public HttpStatusCode? HttpStatusCode { get; set; }
    }
}
