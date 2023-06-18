using MongoDB.Bson;

namespace Domain.MongoDB.Collections._2p;

[BsonCollection("Menu")]
public class Menu : Document
{
    public string Name { get; set; }
    public List<SubMenu> SubMenu { get; set; } = new List<SubMenu>();
}

public class SubMenu
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
}

