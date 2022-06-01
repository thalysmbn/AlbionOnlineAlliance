using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace AlbionOnline.Services.Mongo
{
    public abstract class Document : IDocument
    {
        public Document()
        {
            ObjectId = ObjectId.GenerateNewId();
        }

        [JsonIgnore][BsonElement("_id")] public ObjectId ObjectId { get; set; }
    }
}