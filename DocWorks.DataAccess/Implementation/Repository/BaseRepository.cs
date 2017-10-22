using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Attributes;
using DocWorks.BuildingBlocks.DataAccess.Indexes;

namespace DocWorks.BuildingBlocks.DataAccess.Implementation.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {

        string CollectionName { get; set; }
        public IMongoCollection<T> Collection { get; set; }

        public IQueryable<T> QuerableCollection { get; set; }

        IMongoDatabase db { get; set; }
        IMongoDBClient MongoClient { get; set; }


        public BaseRepository(IDbService dbService)
        {
            // TODO: Check if the client object can be made static, and avoid many open and closures. 
            db = dbService.GetDatabase();
            MongoClient = dbService.GetClient();
            this.CollectionName = CollectionNameAttribute.GetCollectionName<T>();
            this.Collection = GetCollection();
            this.QuerableCollection = this.Collection.AsQueryable<T>();
        }

        public IMongoCollection<T> GetCollection()
        {
            return db.GetCollection<T>(this.CollectionName); 
        }

        public async Task AddDocumentAsync(T document)
        {
            await this.Collection.InsertOneAsync(document);
        }

        public async Task AddDocumentsAsync(List<T> documentList)
        {
            if (documentList.Count > 0)
            {
                await this.Collection.InsertManyAsync(documentList);
            }
        }

        public async Task<List<T>> GetAllDocumentsAsync()
        {
            var filter = new BsonDocument();
            var result = await this.Collection.Find(filter).ToListAsync<T>();

            return result;
        }

        public async Task<T> GetDocumentAsync(string objectIdString)
        {
            var filter = new BsonDocument();
            filter.Add("_id", objectIdString);

            var resultList = await this.Collection.Find(filter).ToListAsync<T>();

            if (resultList.Count == 0)
                return null;
            else return resultList[0];
        }

        public async Task<List<T>> GetAllDocumentsWithSpecificFieldsAsync(string[] projectionFields)
        {
            var filter = new BsonDocument();
            var projBuilder = new ProjectionDefinitionBuilder<T>();
            ProjectionDefinition<T>[] projections = new ProjectionDefinition<T>[projectionFields.Length];
            for (int i = 0; i < projectionFields.Length; i++)
            {
                var prof = projBuilder.Include(projectionFields[i]);
                projections[i] = prof;
            }

            var projectionDefinition = projBuilder.Combine(projections);
            var result = await this.Collection.Find(filter).Project<T>(projectionDefinition).ToListAsync();

            return result;
        }

        public async Task<T> GetDocumentWithSpecificFieldsAsync(string objectIdString, string[] projectionFields)
        {
            // TODO:Mohit Use FilterDefinitionBuilder - as the class is created for specific purpose

            var filter = new BsonDocument();
            filter.Add("_id", objectIdString);

            var projBuilder = new ProjectionDefinitionBuilder<T>();
            ProjectionDefinition<T>[] projections = new ProjectionDefinition<T>[projectionFields.Length];
            for (int i = 0; i < projectionFields.Length; i++)
            {
                var prof = projBuilder.Include(projectionFields[i]);
                projections[i] = prof;
            }

            var projectionDefinition = projBuilder.Combine(projections);
            var resultList = await this.Collection.Find(filter).Project<T>(projectionDefinition).ToListAsync();

            //var resultList = await collection.Find(filter).ToListAsync();

            if (resultList.Count > 0)
            {
                return resultList[0];
            }
            else
            {
                return null;
            }
        }

        public async Task<ReplaceOneResult> ReplaceElementAsync(string _id, T objectToReplace)
        {
            var filter = new BsonDocument();
            filter.Add("_id", _id);
            return await this.Collection.ReplaceOneAsync(filter, objectToReplace, new UpdateOptions { IsUpsert = true });
        }

        public async Task<UpdateResult> UpdateElementAsync<Property>(Property objProperty, string propertyName, string _id)
        {
            var filter = new BsonDocument();
            filter.Add("_id", _id);
            var Update = Builders<T>.Update.Set(propertyName, objProperty);
            return await this.Collection.UpdateOneAsync(filter, Update, new UpdateOptions { IsUpsert = true });
        }

        public async Task<ExpandoObject> UpdateSpecificElementByFilterAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> updateDefination, TField value)
        {
            var objFilterDefination = Builders<T>.Filter.Where(filter);

            var objUpdateDefination = Builders<T>.Update.Set<TField>(updateDefination, value);
            return await this.Collection.FindOneAndUpdateAsync<ExpandoObject>(objFilterDefination, objUpdateDefination);
        }

        public List<T> FindAllDocument(Func<T, bool> predicate)
        {
            // TODO: Pavan - Write Async version. And also another overload with projection
            return this.QuerableCollection.Where(predicate).ToList<T>();
        }

        public async void CreateCollectionAndIndexesIfNotExists(List<DbCollectionIndexCreationOperation> indexCreationList)
        {
            foreach (var indexItem in indexCreationList)
            {
                // construct Index builder
                var indexBuilderObject = Builders<T>.IndexKeys.Ascending(indexItem.PropertyName);
                try
                {
                    await this.Collection.Indexes.CreateOneAsync(indexBuilderObject, indexItem.IndexOption);
                }
                catch (Exception)
                {
                    // Exception comes, if index already exists.
                    // ToDO log
                }
            }
        }
    }
}
