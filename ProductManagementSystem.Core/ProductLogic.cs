using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    public class ProductLogic
    {
        private List<Product> _products = new List<Product>();
        private int _nextId = 1;

        public ProductLogic()
        {
            // Примеры товаров
            AddProduct(new Product { Name = "ноутбук", Description = "мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 });
            AddProduct(new Product { Name = "смартфон", Description = "флагманский телефон", Price = 85000, Category = "Электроника", StockQuantity = 25 });
            AddProduct(new Product { Name = "футболка", Description = "хлопковая футболка", Price = 1500, Category = "Одежда", StockQuantity = 50 });
            AddProduct(new Product { Name = "кроссовки", Description = "спортивные кроссовки", Price = 6500, Category = "Обувь", StockQuantity = 15 });
        }

        public Product AddProduct(Product product)
        {
            product.Id = _nextId++;
            _products.Add(product);
            return product;
        }

        public Product? GetProduct(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public List<Product> GetAllProducts()
        {
            return _products.ToList();
        }

        public bool UpdateProduct(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing == null) return false;
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Category = product.Category;
            existing.StockQuantity = product.StockQuantity;
            return true;
        }

        public bool DeleteProduct(int id)
        {
            var p = _products.FirstOrDefault(x => x.Id == id);
            if (p == null) return false;
            _products.Remove(p);
            return true;
        }

        public List<Product> FilterByCategory(string category)
        {
            return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public decimal CalculateTotalInventoryValue()
        {
            return _products.Sum(p => p.Price * p.StockQuantity);
        }
    }
}
