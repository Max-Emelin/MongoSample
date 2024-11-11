using MongoDB.Bson.Serialization.Attributes;

namespace MongoSample.Entities
{
    [Serializable]
    public class ColorProduct : Product
    {
        [BsonElement("color")]
        public List<string> Colors { get; set; }
    }
}