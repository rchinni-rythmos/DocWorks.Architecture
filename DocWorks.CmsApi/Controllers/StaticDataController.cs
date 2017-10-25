using DocWorks.CMS.Api.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Controllers
{
    [Route("api/[controller]")]
    public class StaticDataController : Controller
    {
        private readonly IStaticDataService _staticDataService = null;

        public StaticDataController(IStaticDataService staticDataService)
        {
            this._staticDataService = staticDataService;
        }

        [HttpGet("{fieldName}")]
        public async Task<IActionResult> Get(string fieldName)
        {
            await Task.Yield();
            return Ok(this._staticDataService.GetStaticData(fieldName));
        }
    }
}
