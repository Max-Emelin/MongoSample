namespace MongoSample.Entities
{
    public class CountryLookedUp : Country
    {
        public List<Province> ProvinceList { get; set; }
    }
}