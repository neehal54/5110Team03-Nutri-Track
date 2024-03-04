using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ContosoCrafts.WebSite.Models;
using Microsoft.AspNetCore.Hosting;

namespace ContosoCrafts.WebSite.Services
{
    public class JsonFileProductService
    {
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        private string JsonFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json");



        public IEnumerable<Product> GetProducts()
        {
            using var jsonFileReader = File.OpenText(JsonFileName);
            return JsonSerializer.Deserialize<Product[]>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public bool AddFeedckToProduct(string id, string message)
        {
            
            var ProductData = GetProducts();
            var ProductToUpdate = ProductData.FirstOrDefault(P => P.Id.Equals(id));

   
            if (ProductToUpdate == null)
            {
                return false;
            }

            //If the feedback field is not present then make it and add the message
            if (ProductToUpdate.Feedback == null)
            {
                ProductToUpdate.Feedback = new string[] { message };
                SaveProductDataToJsonFile(ProductData);
                return true;
            }

            //else append the message in the array

            var feedbacks = ProductToUpdate.Feedback.ToList();
            feedbacks.Add(message);
            ProductToUpdate.Feedback = feedbacks.ToArray();

            //Save the data to json file
            SaveProductDataToJsonFile(ProductData);

            return true;
        }

        private void SaveProductDataToJsonFile(IEnumerable<Product> productData)
        {
            var json = JsonSerializer.Serialize(productData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(JsonFileName, json);
        }
     
        public void AddRating(string productId, int rating)
        {
            var products = GetProducts();

            if (products.First(x => x.Id == productId).Ratings == null)
            {
                products.First(x => x.Id == productId).Ratings = new int[] { rating };
            }
            else
            {
                var ratings = products.First(x => x.Id == productId).Ratings.ToList();
                ratings.Add(rating);
                products.First(x => x.Id == productId).Ratings = ratings.ToArray();
            }

            using var outputStream = File.OpenWrite(JsonFileName);

            JsonSerializer.Serialize<IEnumerable<Product>>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                products
            );
        }
    }
}
