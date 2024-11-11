using MongoDB.Bson.Serialization.Attributes;

namespace MongoSample.Entities
{
    public class Country
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string CountryId { get; set; }

        [BsonElement("country_name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string CountryName { get; set; }

        [BsonElement("country_code"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string CountryCode { get; set; }
    }

    public class CountryLookedUp : Country
    {
        public List<Province> ProvinceList { get; set; }
    }
}