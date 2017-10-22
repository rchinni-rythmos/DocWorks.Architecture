using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.DataAccess.Abstractions
{
     public interface IDbService
    {
         IMongoDBClient  GetClient();
         IMongoDatabase GetDatabase();

    }
}
