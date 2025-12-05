using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// Статус операции добавления товара.
    /// </summary>
    public enum AddProductStatus
    {
        /// <summary>Товар успешно добавлен</summary>
        Success,
        /// <summary>ID уже существует</summary>
        DuplicateId,
        /// <summary>Товар с таким именем и категорией уже существует</summary>
        DuplicateProduct,
        /// <summary>Количество товара увеличено путем слияния</summary>
        Merged,
        /// <summary>Операция отменена пользователем</summary>
        Cancelled,
        /// <summary>Произошла ошибка</summary>
        Error
    }

    /// <summary>
    /// Результат операции добавления товара.
    /// </summary>
    public class AddProductResult
    {
        /// <summary>Статус операции</summary>
        public AddProductStatus Status { get; set; }
        /// <summary>Существующий товар (если найден)</summary>
        public Product? ExistingProduct { get; set; }
        /// <summary>Сообщение о результате</summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Статус операции удаления товара.
    /// </summary>
    public enum DeleteProductStatus
    {
        /// <summary>Операция завершена успешно</summary>
        Success,
        /// <summary>Количество уменьшено</summary>
        QuantityReduced,
        /// <summary>Товар удален полностью</summary>
        DeletedCompletely,
        /// <summary>Запрошено количество больше имеющегося</summary>
        QuantityExceeded,
        /// <summary>Операция отменена</summary>
        Cancelled,
        /// <summary>Произошла ошибка</summary>
        Error
    }

    /// <summary>
    /// Результат операции удаления товара.
    /// </summary>
    public class DeleteProductResult
    {
        /// <summary>Статус операции</summary>
        public DeleteProductStatus Status { get; set; }
        /// <summary>Товар, над которым выполнялась операция</summary>
        public Product? Product { get; set; }
        /// <summary>Оставшееся количество товара</summary>
        public int RemainingQuantity { get; set; }
        /// <summary>Сообщение о результате</summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// SOLID - S: Класс отвечает только за координацию между CRUD и бизнес-функциями.
    /// SOLID - D: Зависит от абстракций IRepository и IBusinessFunctions, а не от конкретных реализаций.
    /// SOLID - O: Можно расширять новыми методами без изменения существующих.
    /// SOLID - L: Реализует интерфейс ILogic.
    /// 
    /// Логика управления товарами.
    /// Координирует работу между репозиторием (CRUD) и бизнес-функциями.
    /// 
    /// Архитектура:
    /// ProductLogic ? IRepository (для CRUD: Add, Delete, Update, ReadAll, ReadById)
    ///             ? IBusinessFunctions (для бизнес-логики: FilterByCategory, GroupByCategory, Calculate и т.д.)
    /// </summary>
    public class ProductLogic : ILogic
    {
        /// <summary>
        /// SOLID - D: Зависимость от абстракции IRepository для CRUD операций.
        /// Репозиторий отвечает за работу с базой данных.
        /// </summary>
        /// 





        /// Л О Г И К А =============================================================================== Л О Г И К А
       
       /// Л О Г И К А =============================================================================== Л О Г И К А
       
        private readonly IRepository<Product>? _repository;

// ==============================================================
        /// <summary>
        /// SOLID - D: Зависимость от абстракции IBusinessFunctions для бизнес-логики.
        /// Бизнес-функции отвечают за фильтрацию, расчёты, группировку.
        /// </summary>
        private readonly IBusinessFunctions _businessFunctions;

        /// <summary>
        /// Локальный список товаров (используется если репозиторий не задан).
        /// </summary>
        private List<Product> _products = new List<Product>();

        /// <summary>
        /// Счётчик для генерации ID при работе без репозитория.
        /// </summary>
        private int _nextId = 1;

        /// <summary>
        /// Конструктор по умолчанию для обратной совместимости.
        /// </summary>
        public ProductLogic() : this(null, new BusinessFunctions())
        {
        }

        /// <summary>
        /// SOLID - D: Constructor Injection - внедрение зависимостей через конструктор.
        /// 
        /// Инициализирует ProductLogic с указанным репозиторием и бизнес-функциями.
        /// 
        /// Ninject создаст и передаст:
        /// - IRepository<Product> ? EntityRepository или DapperRepository (настраивается в SimpleConfigModule)
        /// - IBusinessFunctions ? BusinessFunctions
        /// </summary>
        /// <param name="repository">Репозиторий для CRUD операций (null для работы с локальным списком)</param>
        /// <param name="businessFunctions">Бизнес-функции для фильтрации, расчётов и т.д.</param>
        public ProductLogic(IRepository<Product>? repository, IBusinessFunctions businessFunctions)
        {
            _repository = repository;
            _businessFunctions = businessFunctions;

            if (_repository != null)
            {
                InitializeDataIfEmpty();
            }
            else
            {
                InitializeLocalData();
            }
        }

        /// <summary>
        /// Инициализирует локальный список примерами товаров.
        /// </summary>
        private void InitializeLocalData()
        {
            AddProduct(new Product { Id = 0, Name = "Ноутбук", Description = "Мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 });
            AddProduct(new Product { Id = 0, Name = "Смартфон iPhone 15 Pro", Description = "Флагманский смартфон Apple", Price = 85000, Category = "Электроника", StockQuantity = 15 });
            AddProduct(new Product { Id = 0, Name = "Беспроводная мышь Logitech", Description = "Эргономичная беспроводная мышь", Price = 2500, Category = "Периферия", StockQuantity = 50 });
            AddProduct(new Product { Id = 0, Name = "Механическая клавиатура", Description = "RGB подсветка, Cherry MX switches", Price = 8500, Category = "Периферия", StockQuantity = 20 });
            AddProduct(new Product { Id = 0, Name = "Монитор Samsung 27\"", Description = "4K монитор с IPS матрицей", Price = 35000, Category = "Электроника", StockQuantity = 10 });
            AddProduct(new Product { Id = 0, Name = "Наушники Apple AirPods Pro 2", Description = "Премиум наушники с шумоподавлением", Price = 25000, Category = "Аудио", StockQuantity = 8 });
            AddProduct(new Product { Id = 0, Name = "Веб-камера Logitech C920", Description = "Full HD веб-камера для стриминга", Price = 7500, Category = "Периферия", StockQuantity = 30 });
            AddProduct(new Product { Id = 0, Name = "SSD Samsung 1TB", Description = "Быстрый твердотельный накопитель", Price = 9500, Category = "Комплектующие", StockQuantity = 40 });
            AddProduct(new Product { Id = 0, Name = "Игровая мышь Razer", Description = "Высокоточная мышь для геймеров", Price = 6500, Category = "Периферия", StockQuantity = 25 });
            AddProduct(new Product { Id = 0, Name = "USB Hub 7 портов", Description = "Активный USB 3.0 хаб", Price = 2000, Category = "Аксессуары", StockQuantity = 60 });
        }

        /// <summary>
        /// Инициализирует базу данных примерами товаров, если она пуста.
        /// </summary>
        private void InitializeDataIfEmpty()
        {
            try
            {
                var existingProducts = _repository!.ReadAll().ToList();
                
                // Если база данных пуста, добавляем примеры товаров
                if (!existingProducts.Any())
                {
                    AddProduct(new Product { Id = 0, Name = "Ноутбук", Description = "Мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 });
                    AddProduct(new Product { Id = 0, Name = "Смартфон iPhone 15 Pro", Description = "Флагманский смартфон Apple", Price = 85000, Category = "Электроника", StockQuantity = 15 });
                    AddProduct(new Product { Id = 0, Name = "Беспроводная мышь Logitech", Description = "Эргономичная беспроводная мышь", Price = 2500, Category = "Периферия", StockQuantity = 50 });
                    AddProduct(new Product { Id = 0, Name = "Механическая клавиатура", Description = "RGB подсветка, Cherry MX switches", Price = 8500, Category = "Периферия", StockQuantity = 20 });
                    AddProduct(new Product { Id = 0, Name = "Монитор Samsung 27\"", Description = "4K монитор с IPS матрицей", Price = 35000, Category = "Электроника", StockQuantity = 10 });
                    AddProduct(new Product { Id = 0, Name = "Наушники Apple AirPods Pro 2", Description = "Премиум наушники с шумоподавлением", Price = 25000, Category = "Аудио", StockQuantity = 8 });
                    AddProduct(new Product { Id = 0, Name = "Веб-камера Logitech C920", Description = "Full HD веб-камера для стриминга", Price = 7500, Category = "Периферия", StockQuantity = 30 });
                    AddProduct(new Product { Id = 0, Name = "SSD Samsung 1TB", Description = "Быстрый твердотельный накопитель", Price = 9500, Category = "Комплектующие", StockQuantity = 40 });
                    AddProduct(new Product { Id = 0, Name = "Игровая мышь Razer", Description = "Высокоточная мышь для геймеров", Price = 6500, Category = "Периферия", StockQuantity = 25 });
                    AddProduct(new Product { Id = 0, Name = "USB Hub 7 портов", Description = "Активный USB 3.0 хаб", Price = 2000, Category = "Аксессуары", StockQuantity = 60 });
                }
                else
                {
                    // Обновляем счётчик на основе максимального ID в базе
                    var maxId = existingProducts.Max(p => p.Id);
                    _nextId = maxId + 1;
                }
            }
            catch
            {
                // Если база данных недоступна, используем пустой список
                // Ошибки будут обработаны при следующих операциях
            }
        }

        // CRUD операции - делегируются в IRepository

        /// <summary>
        /// Добавляет новый товар.
        /// CRUD операция ? вызывает _repository.Add()
        /// </summary>
        public Product Add(Product product)
        {
            if (_repository != null)
            {
                _repository.Add(product);
            }
            else
            {
                if (product.Id == 0)
                {
                    product.Id = _nextId++;
                }
                _products.Add(product);
            }

            return product;
        }

        /// <summary>
        /// Получает товар по ID.
        /// CRUD операция ? вызывает _repository.ReadById()
        /// </summary>
        public Product? GetById(int id)
        {
            if (_repository != null)
            {
                return _repository.ReadById(id);
            }
            else
            {
                return _products.FirstOrDefault(p => p.Id == id);
            }
        }

        /// <summary>
        /// Получает все товары.
        /// CRUD операция ? вызывает _repository.ReadAll()
        /// </summary>
        public List<Product> GetAll()
        {
            if (_repository != null)
            {
                return _repository.ReadAll().ToList();
            }
            else
            {
                return _products.ToList();
            }
        }

        /// <summary>
        /// Обновляет товар.
        /// CRUD операция ? вызывает _repository.Update()
        /// </summary>
        public bool Update(Product product)
        {
            try
            {
                if (_repository != null)
                {
                    _repository.Update(product);
                    return true;
                }
                else
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
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаляет товар по ID.
        /// CRUD операция ? вызывает _repository.Delete()
        /// </summary>
        public bool Delete(int id)
        {
            try
            {
                if (_repository != null)
                {
                    _repository.Delete(id);
                    return true;
                }
                else
                {
                    var p = _products.FirstOrDefault(x => x.Id == id);
                    if (p == null) return false;
                    _products.Remove(p);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // ===== Бизнес-функции - делегируются в IBusinessFunctions =====

        /// <summary>
        /// Фильтрует товары по категории.
        /// Бизнес-функция ? вызывает _businessFunctions.FilterByCategory()
        /// </summary>
        public List<Product> FilterByCategory(string category)
        {
            var allProducts = GetAll();
            return _businessFunctions.FilterByCategory(allProducts, category);
        }

        /// <summary>
        /// Группирует товары по категориям.
        /// Бизнес-функция ? вызывает _businessFunctions.GroupByCategory()
        /// </summary>
        public Dictionary<string, List<Product>> GroupByCategory()
        {
            var allProducts = GetAll();
            return _businessFunctions.GroupByCategory(allProducts);
        }

        /// <summary>
        /// Рассчитывает общую стоимость склада.
        /// Бизнес-функция ? вызывает _businessFunctions.CalculateTotalInventoryValue()
        /// </summary>
        public decimal CalculateTotalInventoryValue()
        {
            var allProducts = GetAll();
            return _businessFunctions.CalculateTotalInventoryValue(allProducts);
        }

        /// <summary>
        /// Находит товар по названию и категории.
        /// Бизнес-функция ? вызывает _businessFunctions.FindByNameAndCategory()
        /// </summary>
        public Product? FindProductByNameAndCategory(string name, string category)
        {
            var allProducts = GetAll();
            return _businessFunctions.FindByNameAndCategory(allProducts, name, category);
        }

        /// <summary>
        /// Выполняет поиск товаров.
        /// Бизнес-функция ? вызывает _businessFunctions.Search()
        /// </summary>
        public List<Product> SearchProducts(string query)
        {
            var allProducts = GetAll();
            return _businessFunctions.Search(allProducts, query);
        }

        // Дополнительные методи

        /// <summary>
        /// Увеличивает количество товара.
        /// </summary>
        public bool AddQuantityToProduct(int id, int quantity)
        {
            var product = GetById(id);
            if (product == null) return false;

            product.StockQuantity += quantity;
            return Update(product);
        }

        /// <summary>
        /// Уменьшает количество товара.
        /// </summary>
        public bool RemoveQuantityFromProduct(int id, int quantityToRemove)
        {
            var product = GetById(id);
            if (product == null) return false;

            if (quantityToRemove >= product.StockQuantity)
            {
                return Delete(id);
            }
            else
            {
                product.StockQuantity -= quantityToRemove;
                return Update(product);
            }
        }

        /// <summary>
        /// Добавляет товар с валидацией и возможностью слияния.
        /// </summary>
        public AddProductResult AddProductWithValidation(Product product, bool allowMerge)
        {
            try
            {
                var allProducts = GetAll();

                // Проверка дубликатов по ID
                if (_businessFunctions.ExistsById(allProducts, product.Id))
                {
                    var existingById = GetById(product.Id);
                    return new AddProductResult
                    {
                        Status = AddProductStatus.DuplicateId,
                        ExistingProduct = existingById,
                        Message = $"Товар с ID {product.Id} уже существует"
                    };
                }

                // Проверка дубликатов по названию и категории
                var existingProduct = FindProductByNameAndCategory(product.Name, product.Category);
                if (existingProduct != null)
                {
                    if (product.Category.Equals("Разное", StringComparison.OrdinalIgnoreCase))
                    {
                        Add(product);
                        return new AddProductResult
                        {
                            Status = AddProductStatus.Success,
                            Message = "Товар успешно добавлен"
                        };
                    }

                    if (allowMerge)
                    {
                        AddQuantityToProduct(existingProduct.Id, product.StockQuantity);
                        return new AddProductResult
                        {
                            Status = AddProductStatus.Merged,
                            ExistingProduct = existingProduct,
                            Message = $"Количество увеличено. Новое количество: {existingProduct.StockQuantity}"
                        };
                    }
                    else
                    {
                        return new AddProductResult
                        {
                            Status = AddProductStatus.DuplicateProduct,
                            ExistingProduct = existingProduct,
                            Message = $"Товар с названием '{product.Name}' и категорией '{product.Category}' уже существует"
                        };
                    }
                }

                Add(product);
                return new AddProductResult
                {
                    Status = AddProductStatus.Success,
                    Message = "Товар успешно добавлен"
                };
            }
            catch (Exception ex)
            {
                return new AddProductResult
                {
                    Status = AddProductStatus.Error,
                    Message = $"Ошибка при добавлении товара: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Удаляет указанное количество товара.
        /// </summary>
        public DeleteProductResult DeleteProductByQuantity(int productId, int quantityToDelete)
        {
            try
            {
                var product = GetById(productId);
                if (product == null)
                {
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.Error,
                        Message = "Товар не найден"
                    };
                }

                if (quantityToDelete == 0 || quantityToDelete >= product.StockQuantity)
                {
                    Delete(productId);
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.DeletedCompletely,
                        Product = product,
                        RemainingQuantity = 0,
                        Message = "Товар полностью удален"
                    };
                }

                if (quantityToDelete < 0)
                {
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.Error,
                        Message = "Количество не может быть отрицательным"
                    };
                }

                product.StockQuantity -= quantityToDelete;
                Update(product);

                return new DeleteProductResult
                {
                    Status = DeleteProductStatus.QuantityReduced,
                    Product = product,
                    RemainingQuantity = product.StockQuantity,
                    Message = $"Количество уменьшено. Осталось: {product.StockQuantity}"
                };
            }
            catch (Exception ex)
            {
                return new DeleteProductResult
                {
                    Status = DeleteProductStatus.Error,
                    Message = $"Ошибка при удалении товара: {ex.Message}"
                };
            }
        }

        // Устаревшие методы для обратной совместимости
        // Эти методы делегируют вызовы к новым методам

        #region obsolete methods for backward compatability

        [Obsolete("Используйте Add() вместо AddProduct()")]
        public Product AddProduct(Product product) => Add(product);

        [Obsolete("Используйте GetById() вместо GetProduct()")]
        public Product? GetProduct(int id) => GetById(id);

        [Obsolete("Используйте GetAll() вместо GetAllProducts()")]
        public List<Product> GetAllProducts() => GetAll();

        [Obsolete("Используйте Update() вместо UpdateProduct()")]
        public bool UpdateProduct(Product product) => Update(product);

        [Obsolete("Используйте Delete() вместо DeleteProduct()")]
        public bool DeleteProduct(int id) => Delete(id);

        [Obsolete("Используйте _businessFunctions.ExistsById()")]
        public bool IdExists(int id)
        {
            var allProducts = GetAll();
            return _businessFunctions.ExistsById(allProducts, id);
        }

        [Obsolete("Используйте метод GetAll() с индексацией")]
        public List<(int Index, Product Product)> GetProductsWithIndexes()
        {
            return GetAll().Select((p, index) => (index + 1, p)).ToList();
        }

        [Obsolete("Используйте метод GetAll() с форматированием")]
        public List<(int Index, Product Product, string DisplayText)> GetProductsForDeletionMenu()
        {
            return GetAll().Select((p, index) =>
                (index + 1, p, $"{index + 1}. {p.Name}, {p.StockQuantity} шт")).ToList();
        }
    }
}
#endregion