using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using DocWorks.BuildingBlocks.DataAccess.Abstractions;

namespace DocWorks.BuildingBlocks.DataAccess.Implementation
{
    public class MongoDBClient : IMongoDBClient
    {
        public static IMongoClient mongoClient { get; set; }

        public IMongoDatabase GetConnection(string connectionString, string databaseName)
        {
            if (mongoClient == null)
            {
                mongoClient = new MongoClient(connectionString);

            }
            return mongoClient.GetDatabase(databaseName);    
        }
    }
}
