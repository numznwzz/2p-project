using MongoDB.Bson;

namespace Domain.MongoDB.Collections._2p;

[BsonCollection("machine")]
public class Machine : Document
{
    public string Name { get; set; }
    public int ProductionSize { get; set; }
    public ObjectId FactoryId { get; set; }
}