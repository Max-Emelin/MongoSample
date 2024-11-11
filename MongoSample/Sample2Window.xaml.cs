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

            //LookupFunc(database);
            //ArrayFunc(database);
            
        }
        private static void ArrayFunc(IMongoDatabase database)
        {
            //InsertColorProducts(database);

            var colorProductCollection = database.GetCollection<ColorProduct>("color_product");

            //Find
            var findFilterDefinition = Builders<ColorProduct>.Filter.Empty;
            var findColorProductList = colorProductCollection.Find(findFilterDefinition).ToList();

            //Update
            var updateFilterDefinition = Builders<ColorProduct>.Filter.Eq(cp => cp.ProductId, "673243422155ca4f74dca5be");
            var updateColorProductList = colorProductCollection.Find(updateFilterDefinition).ToList();
            var colors = new List<String> { "Orange", "Purple" };
            var updateDefinition = Builders<ColorProduct>.Update.Set(cp => cp.Colors, colors);

            colorProductCollection.UpdateOne(updateFilterDefinition, updateDefinition);

        }

        private static void UnwindFunc(IMongoDatabase database)
        {
            var colorProductCollection = database.GetCollection<ColorProduct>("color_product");


            //Returns 5 entities with color difference
            var allUnwindResult = colorProductCollection.Aggregate()
                .Unwind<ColorProduct, ColorProductUnwindResult>(cp => cp.Colors)
                .ToList();


            //Filter unwind
            var filterDefinition = Builders<ColorProduct>.Filter.Eq(cp => cp.ProductCode, "001");
            var result = colorProductCollection.Aggregate()
                .Match(filterDefinition)
                .Unwind<ColorProduct, ColorProductUnwindResult>(cp => cp.Colors)
                .ToList();
        }

        private static void LookupFunc(IMongoDatabase database)
        {
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
            InsertProvinces(database);*/
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

        private static void InsertProvinces(IMongoDatabase database)
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

        private static void InsertColorProducts(IMongoDatabase database)
        {
            var colorProductList = new List<ColorProduct>
            {
                new ColorProduct
                {
                    ProductName = "Product 001",
                    ProductCode = "001",
                    Price = 10,
                    Colors = new List<string>
                    {
                        "Black", "White", "Red"
                    }
                },
                new ColorProduct
                {
                    ProductName = "Product 002",
                    ProductCode = "002",
                    Price = 20,
                    Colors = new List<string>
                    {
                        "Green", "Blue", "Yellow"
                    }
                }
            };
            var colorProductCollection = database.GetCollection<ColorProduct>("color_product");

            colorProductCollection.InsertMany(colorProductList);
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
