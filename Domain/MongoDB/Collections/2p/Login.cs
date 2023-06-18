using MongoDB.Bson;

namespace Domain.MongoDB.Collections._2p
{
    [BsonCollection("auth")]
    public class Login : Document
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public ObjectId EmployeeId { get; set; }
        public DateTime LastLogin { get; set; }
        public ObjectId RoleId { get; set; }
    }
}