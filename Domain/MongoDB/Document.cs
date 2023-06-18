using MongoDB.Bson;

namespace Domain.MongoDB
{
    public class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}