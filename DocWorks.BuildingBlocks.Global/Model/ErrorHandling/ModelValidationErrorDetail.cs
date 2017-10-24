using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Model.ErrorHandling
{
    public class ModelValidationErrorDetail
    {
        public string Field { get; set; }
        public List<string> Message { get; set; }
    }
}
