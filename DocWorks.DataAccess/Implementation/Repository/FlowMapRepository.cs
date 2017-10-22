using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;

namespace DocWorks.BuildingBlocks.DataAccess.Implementation.Repository
{
    public class FlowMapRepository : BaseRepository<FlowMap>, IFlowMapRepository
    {
        public FlowMapRepository(IDbService dbService) : base(dbService)
        {
        }
    }
}
