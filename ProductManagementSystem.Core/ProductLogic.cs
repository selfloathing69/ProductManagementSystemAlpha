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
        /// Список всех товаров в системе (используется когда работа идёт без репозитория).
        /// </summary>
        private List<Product> _products = new List<Product>();
        
        /// <summary>
        /// Счётчик для генерации уникальных идентификаторов товаров (используется когда работа идёт без репозитория).
        /// </summary>
        private int _nextId = 1;

        /// <summary>
        /// Репозиторий для работы с данными (если используется).
        /// </summary>
        private dynamic? _repository = null;

        /// <summary>
        /// Флаг, указывающий используется ли репозиторий.
        /// </summary>
        private bool _useRepository = false;

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductLogic и заполняет его примерами товаров.
        /// Используется режим работы с локальным списком в памяти.
        /// </summary>
        public ProductLogic()
        {
            _useRepository = false;
            // Добавление примеров товаров для демонстрации функциональности системы
            AddProduct(new Product { Name = "Ноутбук", Description = "Мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 });
            AddProduct(new Product { Name = "Смартфон", Description = "Флагманский телефон", Price = 85000, Category = "Электроника", StockQuantity = 25 });
            AddProduct(new Product { Name = "Футболка", Description = "Хлопковая футболка", Price = 1500, Category = "Одежда", StockQuantity = 50 });
            AddProduct(new Product { Name = "Кроссовки", Description = "Спортивные кроссовки", Price = 6500, Category = "Обувь", StockQuantity = 15 });
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductLogic с использованием репозитория.
        /// Используется режим работы с базой данных через репозиторий.
        /// </summary>
        /// <param name="repository">Репозиторий для работы с данными</param>
        public ProductLogic(dynamic repository)
        {
            _repository = repository;
            _useRepository = true;
        }

        /// <summary>
        /// Добавляет новый товар в систему.
        /// Автоматически присваивает уникальный идентификатор товару (только для режима без репозитория).
        /// </summary>
        /// <param name="product">Товар для добавления</param>
        /// <returns>Добавленный товар с присвоенным ID</returns>
        public Product AddProduct(Product product)
        {
            if (_useRepository)
            {
                _repository.Add(product);
                return product;
            }
            else
            {
                product.Id = _nextId++;  // Присваиваем уникальный ID и увеличиваем счётчик
                _products.Add(product);  // Добавляем товар в список
                return product;
            }
        }

        /// <summary>
        /// Получает товар по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>Товар с указанным ID или null, если товар не найден</returns>
        public Product? GetProduct(int id)
        {
            if (_useRepository)
            {
                try
                {
                    return _repository.ReadById(id);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return _products.FirstOrDefault(p => p.Id == id);
            }
        }

        /// <summary>
        /// Получает список всех товаров в системе.
        /// </summary>
        /// <returns>Копия списка всех товаров</returns>
        public List<Product> GetAllProducts()
        {
            if (_useRepository)
            {
                return _repository.ReadAll().ToList();
            }
            else
            {
                return _products.ToList();  // Возвращаем копию списка для безопасности
            }
        }

        /// <summary>
        /// Обновляет информацию о существующем товаре.
        /// </summary>
        /// <param name="product">Товар с обновлённой информацией</param>
        /// <returns>true, если товар успешно обновлён; false, если товар не найден</returns>
        public bool UpdateProduct(Product product)
        {
            if (_useRepository)
            {
                try
                {
                    _repository.Update(product);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
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
        }

        /// <summary>
        /// Удаляет товар из системы по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор товара для удаления</param>
        /// <returns>true, если товар успешно удалён; false, если товар не найден</returns>
        public bool DeleteProduct(int id)
        {
            if (_useRepository)
            {
                try
                {
                    _repository.Delete(id);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                var p = _products.FirstOrDefault(x => x.Id == id);
                if (p == null) return false;  // Товар не найден
                _products.Remove(p);  // Удаляем товар из списка
                return true;
            }
        }

        /// <summary>
        /// Фильтрует товары по категории (бизнес-функция).
        /// Поиск выполняется без учёта регистра.
        /// </summary>
        /// <param name="category">Название категории для фильтрации</param>
        /// <returns>Список товаров указанной категории</returns>
        public List<Product> FilterByCategory(string category)
        {
            if (_useRepository)
            {
                return _repository.ReadAll().Where((Product p) => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        /// <summary>
        /// Рассчитывает общую стоимость всех товаров на складе (бизнес-функция).
        /// Учитывает цену товара и его количество на складе.
        /// </summary>
        /// <returns>Общая стоимость всех товаров на складе</returns>
        public decimal CalculateTotalInventoryValue()
        {
            if (_useRepository)
            {
                return _repository.ReadAll().Sum((Product p) => p.Price * p.StockQuantity);
            }
            else
            {
                return _products.Sum(p => p.Price * p.StockQuantity);
            }
        }
    }
}
