using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using DocWorks.CMS.Api.Model.Request;
using DocWorks.CMS.Api.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Controllers
{

    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        public IResponseRepository _responseRepository = null;

        public ProjectController(IResponseRepository responseRepository)
        {
            this._responseRepository = responseRepository;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this._responseRepository.CreateResponseAsync(CMSOperation.GetProjects);
            await this.ProjectOperation.GetProjectsAsync(result);
            BaseApiResponse apiResponse = new BaseApiResponse();
            apiResponse.ResponseId = result._id;
            return Ok(apiResponse);
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return string.Empty;
        }

        // POST: api/Project
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateProjectRequest createProject)
        {
            var result = await this._responseRepository.CreateResponseAsync(CMSOperation.CreateProject);
            await this.ProjectOperation.CreateProjectAsync(createProject, result);
            BaseApiResponse apiResponse = new BaseApiResponse();
            apiResponse.ResponseId = result._id;
            return Ok(apiResponse);
        }

        // PUT: api/Project/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // To validate the RepoUrl for the CreatePreject request
        [HttpPost]
        [Route("ValidateRepository")]
        public async Task<IActionResult> ValidateRepository([FromBody] ValidateRepositoryRequest request)
        {
            var result = await this._responseRepository.CreateResponseAsync(CMSOperation.ValidateRepository);
            await this.ProjectOperation.ValidateRepositoryAsync(request, result);
            BaseApiResponse apiResponse = new BaseApiResponse();
            apiResponse.ResponseId = result._id;
            return Ok(apiResponse);
        }
    }
}
