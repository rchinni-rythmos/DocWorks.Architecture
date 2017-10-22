using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository
{
    public interface IRepository<T> where T : class
    {
        IMongoCollection<T> GetCollection();
        Task AddDocumentAsync(T document);
        Task AddDocumentsAsync(List<T> documentList);
        Task<List<T>> GetAllDocumentsAsync();
        Task<T> GetDocumentAsync(string objectIdString);
        Task<List<T>> GetAllDocumentsWithSpecificFieldsAsync( string[] projectionFields);
        Task<T> GetDocumentWithSpecificFieldsAsync( string objectIdString, string[] projectionFields);

        Task<ReplaceOneResult> ReplaceElementAsync(string _id, T objectToReplace);
        Task<UpdateResult> UpdateElementAsync<Property>(Property objProperty, string propertyName, string _id);
        Task<ExpandoObject> UpdateSpecificElementByFilterAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> updateDefination, TField value);
        List<T> FindAllDocument(Func<T, bool> predicate);
        void CreateCollectionAndIndexesIfNotExists(List<CMSIndexCreationOption> indexCreationList);
    }
}
