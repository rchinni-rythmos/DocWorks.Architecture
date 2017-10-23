using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Implementation.Repository;
using DocWorks.DataAccess.Common.Abstractions.Repository;
using DocWorks.DataAccess.Common.Entity;

namespace DocWorks.DataAccess.Common.Implementation.Repository
{
    public class ResponseRepository : BaseRepository<Response>, IResponseRepository
    {
        public ResponseRepository(IDbService dbService) : base(dbService)
        {
        }
    }
}
