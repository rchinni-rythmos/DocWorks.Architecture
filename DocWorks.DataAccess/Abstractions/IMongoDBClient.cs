using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.DataAccess.Abstractions
{
    public interface IMongoDBClient
    {
         MongoDB.Driver.IMongoDatabase GetConnection(string connectionString, string databaseName);
    }
}
