using MongoDB.Driver;
using MongoSample.Entities;
using System.Configuration;
using System.IO;
using System.Windows;

namespace MongoSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IMongoCollection<Product> productCollection;

        public MainWindow()
        {
            InitializeComponent();

            InitializeDB();

            LoadProductData();
        }

        private void LoadProductData()
        {
            var filterDefenition = Builders<Product>.Filter.Empty;

            productDataGridView.ItemsSource = productCollection.Find(filterDefenition).ToList();
            productCountLabel.Content = $"Count : {productCollection.CountDocuments(filterDefenition)}";
        }

        private void InitializeDB()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionV1"].ConnectionString;
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);

            productCollection = database.GetCollection<Product>("product");
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product
            {
                ProductCode = productCodeTextBox.Text,
                ProductName = productNameTextBox.Text,
                Price = decimal.Parse(productPriceTextBox.Text)
            };

            productCollection.InsertOne(product);

            LoadProductData();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var filterDefinition = Builders<Product>.Filter.Eq(p => p.ProductCode, productCodeTextBox.Text);
            var updateDefiniton = Builders<Product>.Update
                .Set(p => p.ProductName, productNameTextBox.Text)
                .Set(p => p.Price, decimal.Parse(productPriceTextBox.Text));

            productCollection.UpdateOne(filterDefinition, updateDefiniton);

            LoadProductData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var filterDefenition = Builders<Product>.Filter.Eq(p => p.ProductCode, productCodeTextBox.Text);

            productCollection.DeleteOne(filterDefenition);

            LoadProductData();
        }

        private void InsertBulkButton_Click(object sender, RoutedEventArgs e)
        {
            var csvLines = File.ReadAllLines(@"C:\Users\I\Desktop\mo\Products.csv").ToList();
            var products = csvLines.Select(p => new Product
            {
                ProductName = p.Split(',')[0],
                ProductCode = p.Split(',')[1],
                Price = decimal.Parse(p.Split(",")[2])
            }).ToList();

            productCollection.InsertMany(products);

            LoadProductData();
        }

        private void UpdateBulkButton_Click(object sender, RoutedEventArgs e)
        {
            var filterDefinition = Builders<Product>.Filter.Gte(p => p.Price, 50);
            var updateDefinition = Builders<Product>.Update
                .Set(p => p.Price, 300);

            productCollection.UpdateMany(filterDefinition, updateDefinition);

            LoadProductData();
        }

        private void DeleteBulkButton_Click(object sender, RoutedEventArgs e)
        {
            var filterDefinition = Builders<Product>.Filter.Eq(p => p.Price, 300);

            productCollection.DeleteMany(filterDefinition);

            LoadProductData();
        }

        private void UpsertButton_Click(object sender, RoutedEventArgs e)
        {
            var filterDefinition = Builders<Product>.Filter.Eq(p => p.ProductCode, productCodeTextBox.Text);
            var updateDefiniton = Builders<Product>.Update
                .Set(p => p.ProductName, productNameTextBox.Text)
                .Set(p => p.Price, decimal.Parse(productPriceTextBox.Text));

            productCollection.UpdateOne(filterDefinition, updateDefiniton, new UpdateOptions { IsUpsert = true });

            LoadProductData();
        }

        private void FilterExamples()
        {
            //filters
            var filterPriceGreaterOrEqualThen100 = Builders<Product>.Filter.Gte(p => p.Price, 100);
            var filterPriceLessOrEqualThen100 = Builders<Product>.Filter.Lte(p => p.Price, 100);
            var filterPriceLessThen100 = Builders<Product>.Filter.Lt(p => p.Price, 100);
            var filterPriceGreaterThen100 = Builders<Product>.Filter.Gt(p => p.Price, 100);

            var productCodeList = new List<string> { "101", "102", "103" };
            var filterCodeInList = Builders<Product>.Filter.In(p => p.ProductCode, productCodeList);
            var filterCodeNotInList = Builders<Product>.Filter.Nin(p => p.ProductCode, productCodeList);

            var filterCodeEqual = Builders<Product>.Filter.Eq(p => p.ProductCode, "101");
            var filterCodeNotEqual = Builders<Product>.Filter.Ne(p => p.ProductCode, "101");


            // BIG filters
            var filterCodeNotEqualAndGreater100 = Builders<Product>.Filter.Ne(p => p.ProductCode, "101") &
                Builders<Product>.Filter.Gt(p => p.Price, 100);
            var filterCodeNotEqualOrGreater100 = Builders<Product>.Filter.Ne(p => p.ProductCode, "101") |
                Builders<Product>.Filter.Gt(p => p.Price, 100);


            //Update 
            var updateIncrementDefiniton = Builders<Product>.Update
               .Inc(p => p.Price, decimal.Parse(productPriceTextBox.Text)); // + value
            var updateMultyplyDefiniton = Builders<Product>.Update
               .Mul(p => p.Price, decimal.Parse(productPriceTextBox.Text)); // * value
            var updateMaxDefiniton = Builders<Product>.Update
               .Max(p => p.Price, decimal.Parse(productPriceTextBox.Text)); // max(curr || value)
            var updateMinDefiniton = Builders<Product>.Update
               .Min(p => p.Price, decimal.Parse(productPriceTextBox.Text)); // min(curr || value)


            //Find
            var findFilterDefinition = Builders<Product>.Filter.Empty;
            var products = productCollection.Find(findFilterDefinition)
                .Limit(2) //max number records
                .Skip(3)  //skip first {value} records
                .SortBy(p => p.Price) //from min to max
                .SortByDescending(p => p.Price) //from max to min

                .SortByDescending(p => p.Price)
                .ThenByDescending(p => p.ProductCode) // 2x sort condition

                .ToList();

        }

        private void UpdateExamples()
        {
            var findOneAndUpdateOptions = new FindOneAndUpdateOptions<Product>
            { ReturnDocument = ReturnDocument.Before }; // returns old entity version
            //  { ReturnDocument = ReturnDocument.After };  // returns new entity version

            var filterDefinition = Builders<Product>.Filter.Eq(p => p.ProductCode, productCodeTextBox.Text);
            var updateDefiniton = Builders<Product>.Update
                .Set(p => p.ProductName, productNameTextBox.Text)
                .Set(p => p.Price, decimal.Parse(productPriceTextBox.Text));

            var product = productCollection.FindOneAndUpdate(filterDefinition, updateDefiniton, findOneAndUpdateOptions);

            LoadProductData();

            MessageBox.Show($"Name: {product.ProductName} \r\n Price: {product.Price}");
        }

        private void BulkWriteButton_Click(object sender, RoutedEventArgs e)
        {
            var productCodeList = new List<string> { "101", "102", "103", "110", "111", "112" };
            var bulkWriteModelList = new List<WriteModel<Product>>();

            foreach (var productCode in productCodeList)
            {
                var filterDefinition = Builders<Product>.Filter.Eq(p => p.ProductCode, productCode);
                var updateDefinition = Builders<Product>.Update
                    .Set(p => p.ProductName, $"New product {productCode}")
                    .Set(p => p.Price, 40);

                bulkWriteModelList.Add(new UpdateOneModel<Product>(filterDefinition, updateDefinition) { IsUpsert = true });
            }

            productCollection.BulkWrite(bulkWriteModelList);

            LoadProductData();
        }

        private void GetByIdButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = "6731b16a8dc5c20f9c129c51";
            var filterDefinition = Builders<Product>.Filter.Eq(p => p.ProductId, productId);
            var product = productCollection.Find(filterDefinition).FirstOrDefault();

            if (product != null)
                MessageBox.Show($"Product Name: {product.ProductName} | Product Code : {product.ProductCode}");
            else
                MessageBox.Show("No product exists");
        }

        private void CreateIndex()
        {
            var indexKeys = Builders<Product>.IndexKeys;
            var indexList = new List<CreateIndexModel<Product>>
            {
                new CreateIndexModel<Product>
                    (indexKeys.Ascending(p => p.ProductCode), new CreateIndexOptions { Unique= true}),
                new CreateIndexModel<Product>
                    (indexKeys
                        .Descending(p => p.Price)
                        .Ascending(p => p.ProductCode)
                    , new CreateIndexOptions { Name = "Price_Index" })
            };

            productCollection.Indexes.CreateMany(indexList);
        }

        private void LoadCursorDataButtonn_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => await LoadCursorData());
        }

        private async Task LoadCursorData()
        {
            var filterDefinition = Builders<Product>.Filter.Empty;

            using (var cursor = await productCollection.FindAsync
                (filterDefinition, new FindOptions<Product> { BatchSize = 100 }))
            {
                while (await cursor.MoveNextAsync())
                {
                    var productList = cursor.Current.ToList();

                    foreach (var product in productList)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            productListBox.Items.Add(product.ProductName);
                        });
                    }
                }
            }
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            var sample2Window = new Sample2Window();

            sample2Window.Show();
        }
    }
}