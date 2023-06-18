using System.Linq.Expressions;
using System.Reflection;
using Domain.Environments;
using Domain.MongoDB;
using Domain.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repository
{
    public sealed class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IMongoClient client,  IEnvironmentsConfig environmentsConfig)
        {
            var connectionString = environmentsConfig.GetConnectionString("MongoDb");
            //Log.Information($"ConnRepo: {connectionString}");
            var mongoUrl = new MongoUrl(connectionString);
            var database = client.GetDatabase(mongoUrl.DatabaseName);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private static string GetCollectionName(ICustomAttributeProvider documentType)
        {
            return ((BsonCollectionAttribute) documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault()!).CollectionName;
        }

        public IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public IFindFluent<TDocument, TDocument> Find(Expression<Func<TDocument, bool>>? filterExpression)
        {
            return _collection.Find(filterExpression);
        }

        public IFindFluent<TDocument, TDocument> Find(FilterDefinition<TDocument> filter)
        {
            return _collection.Find(filter);
        }

        public IEnumerable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public IMongoCollection<TDocument> GetCollection()
        {
            return _collection;
        }
        
        public TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return _collection.Find(filterExpression).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            return _collection.Find(filter).SingleOrDefaultAsync(cancellationToken)!;
        }

        public async Task<ObjectId> InsertOneAsync(TDocument document, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
            return document.Id;
        }

        public async Task InsertManyAsync(ICollection<TDocument> documents, CancellationToken cancellationToken = default)
        {
            await _collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
        }

        public async Task ReplaceOneAsync(TDocument document, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document, cancellationToken: cancellationToken);
        }

        public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return _collection.FindOneAndDeleteAsync(filterExpression, cancellationToken: cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(filter, cancellationToken);
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            return _collection.FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteManyAsync(filterExpression, cancellationToken);
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> update, bool isUpsert = false,
            CancellationToken cancellationToken = default)
        {
            var options = new UpdateOptions
            {
                IsUpsert = isUpsert
            };
            return _collection.UpdateOneAsync(filterExpression, update, options, cancellationToken);
        }

        public Task<UpdateResult> UpdateManyAsync(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> update, bool isUpsert = false, CancellationToken cancellationToken = default)
        {
            var options = new UpdateOptions
            {
                IsUpsert = isUpsert
            };
            return _collection.UpdateManyAsync(filterExpression, update, options, cancellationToken);
        }

        public IAggregateFluent<TDocument> Aggregate()
        {
            return _collection.Aggregate(new AggregateOptions());
        }
    }
}