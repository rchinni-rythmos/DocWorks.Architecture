using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.ErrorHandling.Model
{
    public class ModelErrorDetail
    {
        public string Field { get; set; }
        public List<string> Message { get; set; }
    }
}
