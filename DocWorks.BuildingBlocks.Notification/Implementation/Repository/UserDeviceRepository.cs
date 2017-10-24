using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Implementation.Repository;
using DocWorks.BuildingBlocks.Notification.Abstractions.Repository;
using DocWorks.BuildingBlocks.Notification.Entity;

namespace DocWorks.BuildingBlocks.Notification.Implementation.Repository
{
    public class UserDeviceRepository: BaseRepository<UserDevices>, IUserDeviceRepository
    {
        public UserDeviceRepository(IDbService dbService) : base(dbService)
        {
        }
    }
}
