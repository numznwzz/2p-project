using MongoDB.Bson;

namespace Domain.MongoDB.Collections._2p;

[BsonCollection("employee")]
public class Employee : Document
{
    public string Fname { get; set; }
    public string Lname { get; set; }
    public string Position { get; set; }
    public string Ssid { get; set; }
    public string Address { get; set; }
    public string Tel { get; set; }
    public string Email { get; set; }
    public ObjectId FactoryId { get; set; }
}