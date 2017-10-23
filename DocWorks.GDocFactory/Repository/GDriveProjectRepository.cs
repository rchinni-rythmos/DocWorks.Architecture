using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Implementation.Repository;
using DocWorks.GDocFactory.Entity;

namespace DocWorks.GDocFactory.Repository
{
    public class GDriveProjectRepository : BaseRepository<GDriveProject>, IGDriveProjectRepository
    {
        public GDriveProjectRepository(IDbService dbService)
            : base(dbService)
        {
        }
    }
}
