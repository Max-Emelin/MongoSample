﻿using MongoDB.Driver;
using MongoSample.Entities;
using System.Collections.ObjectModel;
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
            var products = productCollection.Find(filterDefenition).ToList();
            productDataGridView.ItemsSource = products;
        }

        private void InitializeDB()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
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



        private void FilterExamples()
        {
            var filterPriceGreaterOrEqualThen100 = Builders<Product>.Filter.Gte(p => p.Price, 100);
            var filterPriceLessOrEqualThen100 = Builders<Product>.Filter.Lte(p => p.Price, 100);
            var filterPriceLessThen100 = Builders<Product>.Filter.Lt(p => p.Price, 100);
            var filterPriceGreaterThen100 = Builders<Product>.Filter.Gt(p => p.Price, 100);

            var productCodeList = new List<string> { "101","102","103"};
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
    }
}