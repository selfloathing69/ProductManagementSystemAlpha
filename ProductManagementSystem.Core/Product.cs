using System;

namespace ProductManagementSystem.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }

        public Product() { }

        public Product(int id, string name, string description, decimal price, string category, int stockQuantity)
        {
            Id = id; Name = name; Description = description; Price = price; Category = category; StockQuantity = stockQuantity;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Название: {Name}, Цена: {Price:C}, Категория: {Category}, Количество: {StockQuantity}";
        }
    }
}
