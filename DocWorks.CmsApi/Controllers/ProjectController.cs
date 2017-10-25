using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Model;
using DocWorks.BuildingBlocks.Global.Model.Events;
using DocWorks.CMS.Api.Abstractions;
using DocWorks.CMS.Api.Model.Request;
using DocWorks.CMS.Api.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IResponseGenerator _responseGenerator = null;
        private readonly IEventBusMessagePublisher _eventBusMessagePublisher = null;

        public ProjectController(IResponseGenerator responseGenerator, IEventBusMessagePublisher eventBusMessagePublisher)
        {
            this._responseGenerator = responseGenerator;
            this._eventBusMessagePublisher = eventBusMessagePublisher;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var responseObject = await this._responseGenerator.CreateResponseAsync(CmsOperation.GetProjects);
            BasePayLoad payLoad = new BasePayLoad();
            payLoad.Request = new ExpandoObject();
            SedaEvent sedaEvent = new SedaEvent(responseObject._id, SedaService.Orchestrator, SedaService.CMS, EventType.Request, CmsOperation.GetProjects, Priority.One, payLoad);
            await this._eventBusMessagePublisher.PublishAsync(sedaEvent);
            BaseApiResponse apiResponse = new BaseApiResponse();
            apiResponse.ResponseId = responseObject._id;
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
            var result = await this._responseGenerator.CreateResponseAsync(CmsOperation.CreateProject);
            BasePayLoad payLoad = new BasePayLoad();
            payLoad.Request = createProject;
            SedaEvent sedaEvent = new SedaEvent(result._id, SedaService.Orchestrator, SedaService.CMS, EventType.Request, CmsOperation.CreateProject, Priority.One, payLoad);
            await this._eventBusMessagePublisher.PublishAsync(sedaEvent);
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
            var result = await this._responseGenerator.CreateResponseAsync(CmsOperation.GetProjects);
            BasePayLoad payLoad = new BasePayLoad();
            payLoad.Request = request;
            SedaEvent sedaEvent = new SedaEvent(result._id, SedaService.Orchestrator, SedaService.CMS, EventType.Request, CmsOperation.ValidateRepository, Priority.One, payLoad);
            await this._eventBusMessagePublisher.PublishAsync(sedaEvent);
            BaseApiResponse apiResponse = new BaseApiResponse();
            apiResponse.ResponseId = result._id;
            return Ok(apiResponse);
        }
    }
}
