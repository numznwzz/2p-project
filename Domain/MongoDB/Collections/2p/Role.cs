using MongoDB.Bson;

namespace Domain.MongoDB.Collections._2p;

[BsonCollection("role")]
public class Role : Document
{
    public RoleMenu Menu { get; set; }
    public RoleSubMenu SubMenu { get; set; }
}

public class RoleMenu
{
    public List<ObjectId> Edit { get; set; }
    public List<ObjectId> Create { get; set; }
}

public class RoleSubMenu
{
    public List<ObjectId> Edit { get; set; }
    public List<ObjectId> Create { get; set; }
}