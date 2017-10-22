using DocWorks.CMS.Api.BusinessLogic;
using DocWorks.CMS.Api.Model.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Controllers
{
    [Route("api/[controller]")]
    public class StaticDataController : Controller
    {

        IStaticDataOperations StaticDataOperations { get; set; }
        public StaticDataController(IStaticDataOperations staticDataOperations)
        {
            this.StaticDataOperations = staticDataOperations;
        }

        [HttpGet("{fieldName}")]
        public async Task<IActionResult> Get(string fieldName)
        {
            await Task.Yield();
            return Ok(this.StaticDataOperations.GetStaticData(fieldName));
        }
    }
}
