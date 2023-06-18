using System.Linq.Expressions;
using Domain.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.Repository
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();

        IFindFluent<TDocument, TDocument> Find(Expression<Func<TDocument, bool>> filterExpression);
        IFindFluent<TDocument, TDocument> Find(FilterDefinition<TDocument> filter);

        IEnumerable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression);

        TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);

        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ObjectId> InsertOneAsync(TDocument document, CancellationToken cancellationToken = default);

        Task InsertManyAsync(ICollection<TDocument> documents, CancellationToken cancellationToken = default);

        Task ReplaceOneAsync(TDocument document, CancellationToken cancellationToken = default);

        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default);

        Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default);

        Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

        Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> update,
            bool isUpsert = false,
            CancellationToken cancellationToken = default);
        Task<UpdateResult> UpdateManyAsync(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> update,
            bool isUpsert = false,
            CancellationToken cancellationToken = default);

        IAggregateFluent<TDocument> Aggregate();

        IMongoCollection<TDocument> GetCollection();
    }
}