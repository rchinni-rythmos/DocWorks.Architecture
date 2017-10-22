using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.DataAccess.Implementation
{
    public class DbService : IDbService
    {
        public static IMongoDatabase Database;
        public static IMongoDBClient MongoClient;

        static DbService()
        {
            MongoClient = new MongoDBClient();
            Database = MongoClient.GetConnection(MongoDBSettings.DBConnectionString, MongoDBSettings.DatabaseName);
        }

        public  IMongoDBClient GetClient() { return MongoClient; }
        public  IMongoDatabase GetDatabase() { return Database; }
    }
}
