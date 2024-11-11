using MongoDB.Bson.Serialization.Attributes;

namespace MongoSample.Entities
{
    public class ColorProductUnwindResult : Product
    {
        [BsonElement("colors")]
        public string Color { get; set; }
    }
}