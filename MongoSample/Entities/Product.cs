using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoSample.Entities
{
    [Serializable]
    public class Product
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("product_code"), BsonRepresentation(BsonType.String)]
        public string ProductCode { get; set; }

        [BsonElement("product_name"), BsonRepresentation(BsonType.String)]
        public string ProductName { get; set; }

        [BsonElement("price"), BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }
    }
}