using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Model.ErrorHandling
{
    public class ExceptionDetail
    {
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string InnerExceptionStackTrace { get; set; }
    }
}
