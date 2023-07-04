using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Data.Entity
{
    public class UserProfileEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.Binary)]
        public byte[]? PasswordHash { get; set; }
        
        [BsonRepresentation(BsonType.Binary)]
        public byte[]? PasswordSalt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}