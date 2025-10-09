using System;

namespace ProductManagementSystem.Model
{
    /// <summary>
    /// Представляет товар в системе управления товарами.
    /// Содержит основную информацию о товаре: идентификатор, название, описание, цену, категорию и количество на складе.
    /// </summary>
    public class Product : IDomainObject
    {
        /// <summary>
        /// Уникальный идентификатор товара.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Название товара.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Описание товара.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Цена товара за единицу.
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Категория товара.
        /// </summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>
        /// Количество товара на складе.
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса Product с пустыми значениями.
        /// </summary>
        public Product() { }

        /// <summary>
        /// Инициализирует новый экземпляр класса Product с указанными параметрами.
        /// </summary>
        /// <param name="id">Уникальный идентификатор товара</param>
        /// <param name="name">Название товара</param>
        /// <param name="description">Описание товара</param>
        /// <param name="price">Цена товара</param>
        /// <param name="category">Категория товара</param>
        /// <param name="stockQuantity">Количество на складе</param>
        public Product(int id, string name, string description, decimal price, string category, int stockQuantity)
        {
            Id = id; Name = name; Description = description; Price = price; Category = category; StockQuantity = stockQuantity;
        }

        /// <summary>
        /// Возвращает строковое представление товара с основной информацией.
        /// </summary>
        /// <returns>Строка с информацией о товаре</returns>
        public override string ToString()
        {
            return $"ID: {Id}, Название: {Name}, Цена: {Price:C}, Категория: {Category}, Количество: {StockQuantity}";
        }
    }
}
