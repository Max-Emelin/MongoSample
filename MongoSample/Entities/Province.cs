using MongoDB.Bson.Serialization.Attributes;

namespace MongoSample.Entities
{
    public class Province
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string ProvinceId { get; set; }

        [BsonElement("province_name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string ProvinceName { get; set; }

        [BsonElement("country_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string CountryId { get; set; }
    }
}