using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlbionOnline.Services.Mongo
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId ObjectId { get; set; }
    }
}