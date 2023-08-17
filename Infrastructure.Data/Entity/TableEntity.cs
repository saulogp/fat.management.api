using Infrastructure.Crosscutting.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Data.Entity
{
    public struct TableEntity
    {
        public TableEntity(){}

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid Owner { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string SystemGame { get; set; } = string.Empty;
        public List<Participant>? Participants { get; set; }
        public List<Genre>? Genres { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool Active { get; set; }
        
        public struct Participant
        {
            public Guid Id { get; set; }
            public bool Notified { get; set; }
        }
    }
}