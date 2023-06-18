namespace Domain.MongoDB.Collections._2p;

[BsonCollection("auth")]
public class Factory  : Document
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Tel { get; set; }
}