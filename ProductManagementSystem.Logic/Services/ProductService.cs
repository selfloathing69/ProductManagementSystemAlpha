using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;
using ProductManagementSystem.Logic.Mappers;
using ProductManagementSystem.Logic.Validators;

namespace ProductManagementSystem.Logic.Services
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за бизнес-логику управления товарами.
    /// SOLID - D: Зависит от абстракций (IRepository), а не от конкретных реализаций.
    /// SOLID - O: Класс закрыт для изменений, но открыт для расширений через DI.
    /// 
    /// Сервис для управления товарами.
    /// Реализует бизнес-логику операций CRUD и дополнительную функциональность.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IRepository<Product>? _repository;
        private readonly ProductMapper _mapper;
        private readonly ProductValidator _validator;
        private List<Product> _products = new List<Product>();
        private int _nextId = 1;

        /// <summary>
        /// SOLID - D: Constructor Injection для внедрения зависимостей.
        /// </summary>
        /// <param name="repository">Репозиторий для работы с базой данных (null для in-memory)</param>
        /// <param name="mapper">Маппер для преобразования между моделями</param>
        /// <param name="validator">Валидатор для проверки данных</param>
        public ProductService(IRepository<Product>? repository, ProductMapper mapper, ProductValidator validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;

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
        /// Инициализирует локальные данные тестовыми товарами.
        /// </summary>
        private void InitializeLocalData()
        {
            var testProducts = new[]
            {
                new ProductModel { Id = 0, Name = "Ноутбук", Description = "Мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 },
                new ProductModel { Id = 0, Name = "Смартфон iPhone 15 Pro", Description = "Флагманский смартфон Apple", Price = 85000, Category = "Электроника", StockQuantity = 15 },
                new ProductModel { Id = 0, Name = "Беспроводная мышь Logitech", Description = "Эргономичная беспроводная мышь", Price = 2500, Category = "Аксессуары", StockQuantity = 50 },
                new ProductModel { Id = 0, Name = "Механическая клавиатура", Description = "RGB подсветка, Cherry MX switches", Price = 8500, Category = "Аксессуары", StockQuantity = 20 },
                new ProductModel { Id = 0, Name = "Монитор Samsung 27\"", Description = "4K монитор с IPS матрицей", Price = 35000, Category = "Электроника", StockQuantity = 10 },
                new ProductModel { Id = 0, Name = "Наушники Apple AirPods Pro 2", Description = "Премиум наушники с шумоподавлением", Price = 25000, Category = "Аудио", StockQuantity = 8 },
                new ProductModel { Id = 0, Name = "Веб-камера Logitech C920", Description = "Full HD веб-камера для стриминга", Price = 7500, Category = "Аксессуары", StockQuantity = 30 },
                new ProductModel { Id = 0, Name = "SSD Samsung 1TB", Description = "Быстрый твердотельный накопитель", Price = 9500, Category = "Комплектующие", StockQuantity = 40 },
                new ProductModel { Id = 0, Name = "Игровая мышь Razer", Description = "Программируемая мышь для геймеров", Price = 6500, Category = "Аксессуары", StockQuantity = 25 },
                new ProductModel { Id = 0, Name = "USB Hub 7 портов", Description = "Компактный USB 3.0 хаб", Price = 2000, Category = "Аксессуары", StockQuantity = 60 }
            };

            foreach (var model in testProducts)
            {
                AddProduct(model);
            }
        }

        /// <summary>
        /// Инициализирует базу данных тестовыми товарами, если она пуста.
        /// </summary>
        private void InitializeDataIfEmpty()
        {
            try
            {
                var existingProducts = _repository!.ReadAll().ToList();

                if (!existingProducts.Any())
                {
                    var testProducts = new[]
                    {
                        new ProductModel { Id = 0, Name = "Ноутбук", Description = "Мощный игровой ноутбук", Price = 75000, Category = "Электроника", StockQuantity = 10 },
                        new ProductModel { Id = 0, Name = "Смартфон iPhone 15 Pro", Description = "Флагманский смартфон Apple", Price = 85000, Category = "Электроника", StockQuantity = 15 },
                        new ProductModel { Id = 0, Name = "Беспроводная мышь Logitech", Description = "Эргономичная беспроводная мышь", Price = 2500, Category = "Аксессуары", StockQuantity = 50 },
                        new ProductModel { Id = 0, Name = "Механическая клавиатура", Description = "RGB подсветка, Cherry MX switches", Price = 8500, Category = "Аксессуары", StockQuantity = 20 },
                        new ProductModel { Id = 0, Name = "Монитор Samsung 27\"", Description = "4K монитор с IPS матрицей", Price = 35000, Category = "Электроника", StockQuantity = 10 },
                        new ProductModel { Id = 0, Name = "Наушники Apple AirPods Pro 2", Description = "Премиум наушники с шумоподавлением", Price = 25000, Category = "Аудио", StockQuantity = 8 },
                        new ProductModel { Id = 0, Name = "Веб-камера Logitech C920", Description = "Full HD веб-камера для стриминга", Price = 7500, Category = "Аксессуары", StockQuantity = 30 },
                        new ProductModel { Id = 0, Name = "SSD Samsung 1TB", Description = "Быстрый твердотельный накопитель", Price = 9500, Category = "Комплектующие", StockQuantity = 40 },
                        new ProductModel { Id = 0, Name = "Игровая мышь Razer", Description = "Программируемая мышь для геймеров", Price = 6500, Category = "Аксессуары", StockQuantity = 25 },
                        new ProductModel { Id = 0, Name = "USB Hub 7 портов", Description = "Компактный USB 3.0 хаб", Price = 2000, Category = "Аксессуары", StockQuantity = 60 }
                    };

                    foreach (var model in testProducts)
                    {
                        AddProduct(model);
                    }
                }
                else
                {
                    var maxId = existingProducts.Max(p => p.Id);
                    _nextId = maxId + 1;
                }
            }
            catch
            {
                // При ошибках работы с БД продолжаем с пустым репозиторием
            }
        }

        /// <summary>
        /// Добавляет новый товар в систему.
        /// </summary>
        public ProductModel AddProduct(ProductModel model)
        {
            _validator.ValidateProductModel(model);

            var entity = _mapper.ToEntity(model);

            if (_repository != null)
            {
                _repository.Add(entity);
            }
            else
            {
                if (entity.Id == 0)
                {
                    entity.Id = _nextId++;
                }
                _products.Add(entity);
            }

            return _mapper.ToModel(entity);
        }

        /// <summary>
        /// Обновляет существующий товар.
        /// </summary>
        public bool UpdateProduct(int id, ProductModel model)
        {
            model.Id = id;
            _validator.ValidateProductModel(model);

            try
            {
                var entity = _mapper.ToEntity(model);

                if (_repository != null)
                {
                    _repository.Update(entity);
                    return true;
                }
                else
                {
                    var existing = _products.FirstOrDefault(p => p.Id == id);
                    if (existing == null) return false;

                    existing.Name = entity.Name;
                    existing.Description = entity.Description;
                    existing.Price = entity.Price;
                    existing.Category = entity.Category;
                    existing.StockQuantity = entity.StockQuantity;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаляет товар по его ID.
        /// </summary>
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
        /// Получает товар по его ID.
        /// </summary>
        public ProductModel? GetProduct(int id)
        {
            Product? entity;

            if (_repository != null)
            {
                entity = _repository.ReadById(id);
            }
            else
            {
                entity = _products.FirstOrDefault(p => p.Id == id);
            }

            return entity != null ? _mapper.ToModel(entity) : null;
        }

        /// <summary>
        /// Получает список всех товаров.
        /// </summary>
        public List<ProductModel> GetAllProducts()
        {
            IEnumerable<Product> entities;

            if (_repository != null)
            {
                entities = _repository.ReadAll();
            }
            else
            {
                entities = _products;
            }

            return _mapper.ToModelList(entities);
        }

        /// <summary>
        /// Выполняет поиск товаров по запросу.
        /// </summary>
        public List<ProductModel> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return GetAllProducts();
            }

            IEnumerable<Product> entities;

            if (_repository != null)
            {
                entities = _repository.ReadAll();
            }
            else
            {
                entities = _products;
            }

            var filtered = entities.Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Contains(query, StringComparison.OrdinalIgnoreCase));

            return _mapper.ToModelList(filtered);
        }

        /// <summary>
        /// Фильтрует товары по категории.
        /// </summary>
        public List<ProductModel> FilterByCategory(string category)
        {
            IEnumerable<Product> entities;

            if (_repository != null)
            {
                entities = _repository.ReadAll();
            }
            else
            {
                entities = _products;
            }

            var filtered = entities.Where(p =>
                p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            return _mapper.ToModelList(filtered);
        }

        /// <summary>
        /// Вычисляет общую стоимость всех товаров на складе.
        /// </summary>
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
        /// Добавляет количество товара на склад.
        /// </summary>
        public bool AddQuantityToProduct(int id, int quantity)
        {
            var product = GetProduct(id);
            if (product == null) return false;

            product.StockQuantity += quantity;
            return UpdateProduct(id, product);
        }

        /// <summary>
        /// Удаляет указанное количество товара со склада.
        /// </summary>
        public DeleteProductResult DeleteProductByQuantity(int id, int quantityToRemove)
        {
            try
            {
                var product = GetProduct(id);
                if (product == null)
                {
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.Error,
                        Message = "Товар не найден"
                    };
                }

                if (quantityToRemove == 0 || quantityToRemove >= product.StockQuantity)
                {
                    DeleteProduct(id);
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.DeletedCompletely,
                        Product = _mapper.ToEntity(product),
                        RemainingQuantity = 0,
                        Message = "Товар полностью удален"
                    };
                }

                if (quantityToRemove < 0)
                {
                    return new DeleteProductResult
                    {
                        Status = DeleteProductStatus.Error,
                        Message = "Количество не может быть отрицательным"
                    };
                }

                product.StockQuantity -= quantityToRemove;
                UpdateProduct(id, product);

                return new DeleteProductResult
                {
                    Status = DeleteProductStatus.QuantityReduced,
                    Product = _mapper.ToEntity(product),
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
        /// Добавляет товар с проверкой дубликатов и возможностью объединения.
        /// </summary>
        public AddProductResult AddProductWithValidation(ProductModel model, bool allowMerge)
        {
            try
            {
                _validator.ValidateProductModel(model);

                // Проверка существования ID
                var existingById = GetProduct(model.Id);
                if (existingById != null)
                {
                    return new AddProductResult
                    {
                        Status = AddProductStatus.DuplicateId,
                        ExistingProduct = _mapper.ToEntity(existingById),
                        Message = $"Товар с ID {model.Id} уже существует"
                    };
                }

                // Проверка существования товара с такими же именем и категорией
                var existingProduct = FindProductByNameAndCategory(model.Name, model.Category);
                if (existingProduct != null)
                {
                    // Не объединяем для категории "Разное"
                    if (model.Category.Equals("Разное", StringComparison.OrdinalIgnoreCase))
                    {
                        AddProduct(model);
                        return new AddProductResult
                        {
                            Status = AddProductStatus.Success,
                            Message = "Товар успешно добавлен"
                        };
                    }

                    if (allowMerge)
                    {
                        AddQuantityToProduct(existingProduct.Id, model.StockQuantity);
                        return new AddProductResult
                        {
                            Status = AddProductStatus.Merged,
                            ExistingProduct = _mapper.ToEntity(existingProduct),
                            Message = $"Количество увеличено. Новое количество: {existingProduct.StockQuantity}"
                        };
                    }
                    else
                    {
                        return new AddProductResult
                        {
                            Status = AddProductStatus.DuplicateProduct,
                            ExistingProduct = _mapper.ToEntity(existingProduct),
                            Message = $"Товар с названием '{model.Name}' и категорией '{model.Category}' уже существует"
                        };
                    }
                }

                // Добавляем новый товар
                AddProduct(model);
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
        /// Находит товар с такими же именем и категорией.
        /// </summary>
        private ProductModel? FindProductByNameAndCategory(string name, string category)
        {
            IEnumerable<Product> entities;

            if (_repository != null)
            {
                entities = _repository.ReadAll();
            }
            else
            {
                entities = _products;
            }

            var entity = entities.FirstOrDefault(p =>
                p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            return entity != null ? _mapper.ToModel(entity) : null;
        }
    }
}
