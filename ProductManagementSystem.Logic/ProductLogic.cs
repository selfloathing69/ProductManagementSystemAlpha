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
    /// SOLID - S: Класс отвечает только за бизнес-логику управления товарами.
    /// SOLID - D: Зависит от абстракции IRepository, а не от конкретной реализации.
    /// SOLID - O: Можно расширять новыми методами без изменения существующих.
    /// 
    /// Содержит бизнес-логику для управления товарами.
    /// Предоставляет функциональность для создания, чтения, обновления и удаления товаров (CRUD операции),
    /// а также дополнительные бизнес-функции для фильтрации и расчёта общей стоимости склада.
    ///     /// Тут мы применили принцип Dependency Inversion Principle (DIP) из SOLID:
    /// - Класс зависит от абстракции (IRepository<Product>), а не от конкретной реализации
    /// - Зависимость внедряется через конструктор (Constructor Injection)
    /// - Это позволяет легко переключаться между различными реализациями репозитория (EF, Dapper)
    /// </summary>
    public class ProductLogic
    {
        /// <summary>
        /// Репозиторий для работы с товарами в базе данных.
        /// Тут мы сделали поле зависимостью от интерфейса, а не от конкретной реализации.
        /// Это ключевой момент DIP - "зависим от абстракций, а не от конкретики".
        /// SOLID - D: Зависимость от интерфейса IRepository, а не от конкретной реализации (EF или Dapper).
        /// </summary>
        private readonly IRepository<Product>? _repository;
        
        /// <summary>
        /// Локальный список товаров (используется если репозиторий не задан).
        /// </summary>
        private List<Product> _products = new List<Product>();
        
        /// <summary>
        /// Счётчик для генерации уникальных идентификаторов товаров при работе с локальным списком.
        /// </summary>
        private int _nextId = 1;

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductLogic без репозитория (использует локальный список).
        /// Тут мы оставили конструктор по умолчанию для обратной совместимости.
        /// </summary>
        public ProductLogic() : this(null)
        {
        }

        /// <summary>
        /// SOLID - D: Constructor Injection - внедрение зависимости через конструктор.
        /// SOLID - L: Поддерживает любую реализацию IRepository без нарушения поведения.
        /// 
        /// Инициализирует новый экземпляр класса ProductLogic с указанным репозиторием.
        ///         /// Тут мы реализовали Constructor Injection - один из способов внедрения зависимостей:
        /// - Конструктор принимает зависимость (IRepository) в качестве параметра
        /// - Зависимость сохраняется в readonly поле, гарантируя неизменность после создания объекта
        /// - Это делает зависимости явными и обязательными
        /// - DI-контейнер (Ninject) автоматически создаст и передаст нужную реализацию репозитория
        /// </summary>
        /// <param name="repository">Репозиторий для работы с товарами (null для использования локального списка)</param>
        public ProductLogic(IRepository<Product>? repository)
        {
            // SOLID - D: Сохранение внедрённой зависимости
            _repository = repository;
            
            // Инициализация данных
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

        /// <summary>
        /// Добавляет новый товар в систему.
        /// ID будет присвоен базой данных автоматически (если используется репозиторий) или генерируется локально.
        /// </summary>
        /// <param name="product">Товар для добавления</param>
        /// <returns>Добавленный товар с присвоенным ID</returns>
        public Product AddProduct(Product product)
        {
            if (_repository != null)
            {
                // Если Id не задан (0), база данных присвоит его автоматически
                _repository.Add(product);
            }
            else
            {
                // Для локального списка генерируем ID сами
                if (product.Id == 0)
                {
                    product.Id = _nextId++;
                }
                _products.Add(product);
            }
            
            return product;
        }

        /// <summary>
        /// Получает товар по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>Товар с указанным ID или null, если товар не найден</returns>
        public Product? GetProduct(int id)
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
        /// Получает список всех товаров в системе.
        /// </summary>
        /// <returns>Список всех товаров</returns>
        public List<Product> GetAllProducts()
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
        /// Обновляет информацию о существующем товаре.
        /// </summary>
        /// <param name="product">Товар с обновлённой информацией</param>
        /// <returns>true, если товар успешно обновлён; false, если товар не найден</returns>
        public bool UpdateProduct(Product product)
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
        /// Удаляет товар из системы по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор товара для удаления</param>
        /// <returns>true, если товар успешно удалён; false, если товар не найден</returns>
        public bool DeleteProduct(int id)
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

        /// <summary>
        /// Фильтрует товары по категории (бизнес-функция).
        /// Поиск выполняется без учёта регистра.
        /// </summary>
        /// <param name="category">Название категории для фильтрации</param>
        /// <returns>Список товаров указанной категории</returns>
        public List<Product> FilterByCategory(string category)
        {
            if (_repository != null)
            {
                return _repository.ReadAll()
                    .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                return _products
                    .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        /// <summary>
        /// Рассчитывает общую стоимость всех товаров на складе (бизнес-функция).
        /// Учитывает цену товара и его количество на складе.
        /// </summary>
        /// <returns>Общая стоимость всех товаров на складе</returns>
        public decimal CalculateTotalInventoryValue()
        {
            if (_repository != null)
            {
                return _repository.ReadAll().Sum(p => p.Price * p.StockQuantity);
            }
            else
            {
                return _products.Sum(p => p.Price * p.StockQuantity);
            }
        }

        /// <summary>
        /// Проверяет, существует ли товар с указанным ID.
        /// </summary>
        /// <param name="id">ID для проверки</param>
        /// <returns>true, если товар с таким ID существует</returns>
        public bool IdExists(int id)
        {
            if (_repository != null)
            {
                return _repository.ReadById(id) != null;
            }
            else
            {
                return _products.Any(p => p.Id == id);
            }
        }

        /// <summary>
        /// Находит товар с таким же названием и категорией.
        /// </summary>
        /// <param name="name">Название товара</param>
        /// <param name="category">Категория товара</param>
        /// <returns>Товар с такими же названием и категорией или null</returns>
        public Product? FindProductByNameAndCategory(string name, string category)
        {
            if (_repository != null)
            {
                return _repository.ReadAll().FirstOrDefault(p => 
                    p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
                    p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return _products.FirstOrDefault(p => 
                    p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
                    p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
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
            if (_repository != null)
            {
                return _repository.ReadAll().Select((p, index) => (index + 1, p)).ToList();
            }
            else
            {
                return _products.Select((p, index) => (index + 1, p)).ToList();
            }
        }

        /// <summary>
        /// Добавляет товар с валидацией и возможностью слияния.
        /// </summary>
        /// <param name="product">Товар для добавления</param>
        /// <param name="allowMerge">Разрешить ли слияние с существующим товаром</param>
        /// <returns>Результат операции добавления</returns>
        public AddProductResult AddProductWithValidation(Product product, bool allowMerge)
        {
            try
            {
                // Проверка существования ID
                if (IdExists(product.Id))
                {
                    var existingById = GetProduct(product.Id);
                    return new AddProductResult
                    {
                        Status = AddProductStatus.DuplicateId,
                        ExistingProduct = existingById,
                        Message = $"Товар с ID {product.Id} уже существует"
                    };
                }

                // Проверка существования товара с таким же именем и категорией
                var existingProduct = FindProductByNameAndCategory(product.Name, product.Category);
                if (existingProduct != null)
                {
                    // Не суммируем для категории "Разное"
                    if (product.Category.Equals("Разное", StringComparison.OrdinalIgnoreCase))
                    {
                        // Добавляем как новый товар
                        AddProduct(product);
                        return new AddProductResult
                        {
                            Status = AddProductStatus.Success,
                            Message = "Товар успешно добавлен"
                        };
                    }

                    if (allowMerge)
                    {
                        // Суммируем количество
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

                // Добавляем новый товар
                AddProduct(product);
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
        /// Удаляет указанное количество товара по ID.
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <param name="quantityToDelete">Количество для удаления (0 = удалить всё)</param>
        /// <returns>Результат операции удаления</returns>
        public DeleteProductResult DeleteProductByQuantity(int productId, int quantityToDelete)
        {
            try
            {
                var product = GetProduct(productId);
                if (product == null)
                {
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.Error,
                        Message = "Товар не найден"
                    };
                }

                // Если quantityToDelete = 0, удаляем всё
                if (quantityToDelete == 0 || quantityToDelete >= product.StockQuantity)
                {
                    DeleteProduct(productId);
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

                // Уменьшаем количество
                product.StockQuantity -= quantityToDelete;
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

        /// <summary>
        /// Получает список товаров для меню удаления.
        /// </summary>
        /// <returns>Список товаров с индексами и форматированным описанием</returns>
        public List<(int Index, Product Product, string DisplayText)> GetProductsForDeletionMenu()
        {
            if (_repository != null)
            {
                return _repository.ReadAll().Select((p, index) => 
                    (index + 1, p, $"{index + 1}. {p.Name}, {p.StockQuantity} шт")).ToList();
            }
            else
            {
                return _products.Select((p, index) => 
                    (index + 1, p, $"{index + 1}. {p.Name}, {p.StockQuantity} шт")).ToList();
            }
        }
    }
}
