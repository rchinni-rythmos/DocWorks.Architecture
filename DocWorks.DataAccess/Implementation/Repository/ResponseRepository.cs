using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;

namespace DocWorks.BuildingBlocks.DataAccess.Implementation.Repository
{
    public class ResponseRepository : BaseRepository<Response>, IResponseRepository
    {
        public ResponseRepository(IDbService dbService) : base(dbService)
        {
        }
    }
}
