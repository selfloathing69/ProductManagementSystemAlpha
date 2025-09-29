using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// Содержит бизнес-логику для управления товарами.
    /// Предоставляет функциональность для создания, чтения, обновления и удаления товаров (CRUD операции),
    /// а также дополнительные бизнес-функции для фильтрации и расчёта общей стоимости склада.
    /// </summary>
    public class ProductLogic
    {
        /// <summary>
        /// Список всех товаров в системе.
        /// </summary>
        private List<Product> _products = new List<Product>();
        
        /// <summary>
        /// Счётчик для генерации уникальных внутренних номеров товаров.
        /// </summary>
        private int _nextNumber = 1;

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductLogic и заполняет его примерами товаров.
        /// </summary>
        public ProductLogic()
        {
            // Добавление примеров товаров для демонстрации функциональности системы
            AddProduct(new Product { Id = 1, Name = "Ноутбук", Description = "Мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 });
            AddProduct(new Product { Id = 2, Name = "Смартфон", Description = "Флагманский телефон", Price = 85000, Category = "Электроника", StockQuantity = 25 });
            AddProduct(new Product { Id = 3, Name = "Футболка", Description = "Хлопковая футболка", Price = 1500, Category = "Одежда", StockQuantity = 50 });
            AddProduct(new Product { Id = 4, Name = "Кроссовки", Description = "Спортивные кроссовки", Price = 6500, Category = "Обувь", StockQuantity = 15 });
        }

        /// <summary>
        /// Добавляет новый товар в систему.
        /// Присваивает внутренний номер товару, ID должен быть предварительно установлен.
        /// </summary>
        /// <param name="product">Товар для добавления</param>
        /// <returns>Добавленный товар с присвоенным номером</returns>
        public Product AddProduct(Product product)
        {
            product.Number = _nextNumber++;  // Присваиваем уникальный внутренний номер и увеличиваем счётчик
            _products.Add(product);  // Добавляем товар в список
            return product;
        }

        /// <summary>
        /// Получает товар по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>Товар с указанным ID или null, если товар не найден</returns>
        public Product? GetProduct(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Получает список всех товаров в системе.
        /// </summary>
        /// <returns>Копия списка всех товаров</returns>
        public List<Product> GetAllProducts()
        {
            return _products.ToList();  // Возвращаем копию списка для безопасности
        }

        /// <summary>
        /// Обновляет информацию о существующем товаре.
        /// </summary>
        /// <param name="product">Товар с обновлённой информацией</param>
        /// <returns>true, если товар успешно обновлён; false, если товар не найден</returns>
        public bool UpdateProduct(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing == null) return false;  // Товар не найден
            
            // Обновляем все поля существующего товара
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Category = product.Category;
            existing.StockQuantity = product.StockQuantity;
            return true;
        }

        /// <summary>
        /// Удаляет товар из системы по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор товара для удаления</param>
        /// <returns>true, если товар успешно удалён; false, если товар не найден</returns>
        public bool DeleteProduct(int id)
        {
            var p = _products.FirstOrDefault(x => x.Id == id);
            if (p == null) return false;  // Товар не найден
            _products.Remove(p);  // Удаляем товар из списка
            return true;
        }

        /// <summary>
        /// Фильтрует товары по категории (бизнес-функция).
        /// Поиск выполняется без учёта регистра.
        /// </summary>
        /// <param name="category">Название категории для фильтрации</param>
        /// <returns>Список товаров указанной категории</returns>
        public List<Product> FilterByCategory(string category)
        {
            return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Рассчитывает общую стоимость всех товаров на складе (бизнес-функция).
        /// Учитывает цену товара и его количество на складе.
        /// </summary>
        /// <returns>Общая стоимость всех товаров на складе</returns>
        public decimal CalculateTotalInventoryValue()
        {
            return _products.Sum(p => p.Price * p.StockQuantity);
        }

        /// <summary>
        /// Проверяет, существует ли товар с указанным ID.
        /// </summary>
        /// <param name="id">ID для проверки</param>
        /// <returns>true, если товар с таким ID существует</returns>
        public bool IdExists(int id)
        {
            return _products.Any(p => p.Id == id);
        }

        /// <summary>
        /// Находит товар с таким же названием и категорией.
        /// </summary>
        /// <param name="name">Название товара</param>
        /// <param name="category">Категория товара</param>
        /// <returns>Товар с такими же названием и категорией или null</returns>
        public Product? FindProductByNameAndCategory(string name, string category)
        {
            return _products.FirstOrDefault(p => 
                p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
                p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Увеличивает количество товара на указанное значение.
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="quantity">Количество для добавления</param>
        /// <returns>true, если операция успешна</returns>
        public bool AddQuantityToProduct(int id, int quantity)
        {
            var product = GetProduct(id);
            if (product == null) return false;
            
            product.StockQuantity += quantity;
            return true;
        }

        /// <summary>
        /// Уменьшает количество товара на указанное значение.
        /// Если количество становится <= 0, товар удаляется.
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="quantityToRemove">Количество для удаления</param>
        /// <returns>true, если операция успешна</returns>
        public bool RemoveQuantityFromProduct(int id, int quantityToRemove)
        {
            var product = GetProduct(id);
            if (product == null) return false;
            
            if (quantityToRemove >= product.StockQuantity)
            {
                // Если удаляем всё или больше - удаляем товар полностью
                return DeleteProduct(id);
            }
            else
            {
                product.StockQuantity -= quantityToRemove;
                return true;
            }
        }

        /// <summary>
        /// Получает список всех товаров с их порядковыми номерами для выбора.
        /// </summary>
        /// <returns>Список кортежей (номер, товар)</returns>
        public List<(int Index, Product Product)> GetProductsWithIndexes()
        {
            return _products.Select((p, index) => (index + 1, p)).ToList();
        }
    }
}
