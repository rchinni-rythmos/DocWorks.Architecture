using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Implementation.Repository;
using DocWorks.DataAccess.Common.Abstractions.Repository;
using DocWorks.DataAccess.Common.Entity;

namespace DocWorks.DataAccess.Common.Implementation.Repository
{
    public class FlowMapRepository : BaseRepository<FlowMap>, IFlowMapRepository
    {
        public FlowMapRepository(IDbService dbService) : base(dbService)
        {
        }
    }
}
