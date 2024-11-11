using MongoDB.Driver;
using MongoSample.Entities;
using System.Configuration;
using System.Windows;

namespace MongoSample
{
    /// <summary>
    /// Interaction logic for Sample2Window.xaml
    /// </summary>
    public partial class Sample2Window : Window
    {
        public Sample2Window()
        {
            InitializeComponent();

            var database = ConnectDatabase();

            //InserValues(database);

            var countryCollection = database.GetCollection<Country>("country");
            var provinceCollection = database.GetCollection<Province>("province");

            //Only connection (in 2 countries : 3 & 2 provinces)
            var baseResult = countryCollection.Aggregate()
                .Lookup<Country, Province, CountryLookedUp>
                    (provinceCollection, c => c.CountryId, p => p.CountryId, clu => clu.ProvinceList)
                .ToList();

            //5 new entities
            var result = countryCollection.Aggregate()
                .Lookup<Country, Province, CountryLookedUp>
                    (provinceCollection, c => c.CountryId, p => p.CountryId, clu => clu.ProvinceList)
                .ToEnumerable()
                .SelectMany(clu => clu.ProvinceList.Select(p => new
                {
                    clu.CountryId,
                    clu.CountryCode,
                    clu.CountryName,
                    p.ProvinceId,
                    p.ProvinceName
                }))
                .ToList();


        }

        private static void InserValues(IMongoDatabase database)
        {
            //US - 67323a67004227a49e8c8ce1
            //UK - 67323a67004227a49e8c8ce2

            /*InsertCountries(database);
            InsertProvince(database);*/
        }

        private static void InsertCountries(IMongoDatabase database)
        {
            var countryList = new List<Country>()
            {
                //US - 67323a67004227a49e8c8ce1
                //UK - 67323a67004227a49e8c8ce2
                new Country
                {
                    CountryCode = "US",
                    CountryName = "United States"
                },
                new Country
                {
                    CountryCode = "UK",
                    CountryName = "United Kingdom"
                }
            };
            var countryCollection = database.GetCollection<Country>("country");

            countryCollection.InsertMany(countryList);
        }

        private static void InsertProvince(IMongoDatabase database)
        {
            var provinceList = new List<Province>()
            {
                new Province
                {
                    ProvinceName = "California",
                    CountryId = "67323a67004227a49e8c8ce1"
                },
                new Province
                {
                    ProvinceName = "Texas",
                    CountryId = "67323a67004227a49e8c8ce1"
                },
                new Province
                {
                    ProvinceName = "Florida",
                    CountryId = "67323a67004227a49e8c8ce1"
                },
                new Province
                {
                    ProvinceName = "Scotland",
                    CountryId = "67323a67004227a49e8c8ce2"
                },
                new Province
                {
                    ProvinceName = "Wales",
                    CountryId = "67323a67004227a49e8c8ce2"
                }
            };

            var provinceCollection = database.GetCollection<Province>("province");

            provinceCollection.InsertMany(provinceList);
        }

        private static IMongoDatabase ConnectDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionV2"].ConnectionString;
            var mongoClient = new MongoClient(connectionString);
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;

            return mongoClient.GetDatabase(databaseName);
        }
    }
}
