using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocWorks.BuildingBlocks.DataAccess.Entity;

namespace DocWorks.CMS.Api.Controllers
{
    [Route("api/[controller]")]
    public class ResponseController : Controller
    {
        private readonly IResponseRepository _responseRepository = null;

        public ResponseController(IResponseRepository responseRepository)
        {
            this._responseRepository = responseRepository;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var response = await this._responseRepository.GetDocumentAsync(id);
            return Ok(response);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
