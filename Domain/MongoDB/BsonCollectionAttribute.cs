namespace Domain.MongoDB
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute : Attribute
    {
        public string? CollectionName { get; }

        public BsonCollectionAttribute(string? collectionName)
        {
            if (collectionName == null || string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException(nameof(collectionName));
            }

            CollectionName = collectionName;
        }
    }
}