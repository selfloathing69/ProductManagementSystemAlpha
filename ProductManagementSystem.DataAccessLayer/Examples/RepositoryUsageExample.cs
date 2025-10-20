using System;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer.Examples
{
    /// <summary>
    /// Примеры использования репозиториев Entity Framework и Dapper.
    /// </summary>
    public static class RepositoryUsageExample
    {
        /// <summary>
        /// Демонстрирует использование Entity Framework репозитория.
        /// </summary>
        public static void EntityFrameworkExample()
        {
            Console.WriteLine("=== Пример использования Entity Framework ===");
            
            try
            {
                // Создание репозитория Entity Framework
                IRepository<Product> efRepository = new EntityRepository<Product>();
                
                // Создание экземпляра ProductLogic с EF репозиторием
                var productLogic = new ProductLogic(efRepository);
                
                Console.WriteLine("Репозиторий Entity Framework создан и готов к использованию.");
                
                // Пример добавления товара
                var newProduct = new Product
                {
                    Id = 0, // ID будет назначен автоматически базой данных
                    Name = "Тестовый товар (EF)",
                    Description = "Товар добавленный через Entity Framework",
                    Price = 1500,
                    Category = "Тестовая",
                    StockQuantity = 5
                };
                
                Console.WriteLine("Добавление нового товара...");
                productLogic.AddProduct(newProduct);
                Console.WriteLine($"Товар добавлен с ID: {newProduct.Id}");
                
                // Получение всех товаров
                var products = productLogic.GetAllProducts();
                Console.WriteLine($"Всего товаров в базе: {products.Count}");
                
                Console.WriteLine("Entity Framework работает корректно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при работе с Entity Framework: {ex.Message}");
                Console.WriteLine("Убедитесь, что база данных создана и доступна.");
            }
        }
        
        /// <summary>
        /// Демонстрирует использование Dapper репозитория.
        /// </summary>
        public static void DapperExample()
        {
            Console.WriteLine("\n=== Пример использования Dapper ===");
            
            try
            {
                // Создание репозитория Dapper
                IRepository<Product> dapperRepository = new DapperRepository<Product>();
                
                // Создание экземпляра ProductLogic с Dapper репозиторием
                var productLogic = new ProductLogic(dapperRepository);
                
                Console.WriteLine("Репозиторий Dapper создан и готов к использованию.");
                
                // Пример добавления товара
                var newProduct = new Product
                {
                    Id = 0, // ID будет назначен автоматически
                    Name = "Тестовый товар (Dapper)",
                    Description = "Товар добавленный через Dapper",
                    Price = 2500,
                    Category = "Тестовая",
                    StockQuantity = 3
                };
                
                Console.WriteLine("Добавление нового товара...");
                productLogic.AddProduct(newProduct);
                Console.WriteLine($"Товар добавлен с ID: {newProduct.Id}");
                
                // Получение всех товаров
                var products = productLogic.GetAllProducts();
                Console.WriteLine($"Всего товаров в базе: {products.Count}");
                
                Console.WriteLine("Dapper работает корректно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при работе с Dapper: {ex.Message}");
                Console.WriteLine("Убедитесь, что база данных создана и доступна.");
            }
        }
        
        /// <summary>
        /// Демонстрирует использование репозитория с пользовательской строкой подключения.
        /// </summary>
        public static void CustomConnectionStringExample()
        {
            Console.WriteLine("\n=== Пример с пользовательской строкой подключения ===");
            
            var customConnectionString = "Server=AspireNotebook\\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;";
            
            try
            {
                // Создание Dapper репозитория с пользовательской строкой подключения
                var dapperRepository = new DapperRepository<Product>(customConnectionString);
                var productLogic = new ProductLogic(dapperRepository);
                
                Console.WriteLine("Репозиторий с пользовательской строкой подключения создан.");
                
                var products = productLogic.GetAllProducts();
                Console.WriteLine($"Подключение успешно! Товаров в базе: {products.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Демонстрирует CRUD операции через репозиторий.
        /// </summary>
        public static void CrudOperationsExample()
        {
            Console.WriteLine("\n=== Пример CRUD операций ===");
            
            try
            {
                IRepository<Product> repository = new EntityRepository<Product>();
                var productLogic = new ProductLogic(repository);
                
                // CREATE - Создание
                Console.WriteLine("\n1. CREATE - Создание товара");
                var product = new Product
                {
                    Id = 0,
                    Name = "CRUD Test Product",
                    Description = "Товар для демонстрации CRUD",
                    Price = 999,
                    Category = "Тест",
                    StockQuantity = 10
                };
                productLogic.AddProduct(product);
                Console.WriteLine($"Товар создан с ID: {product.Id}");
                
                // READ - Чтение
                Console.WriteLine("\n2. READ - Чтение товара");
                var readProduct = productLogic.GetProduct(product.Id);
                if (readProduct != null)
                {
                    Console.WriteLine($"Товар найден: {readProduct.Name}, Цена: {readProduct.Price}");
                }
                
                // UPDATE - Обновление
                Console.WriteLine("\n3. UPDATE - Обновление товара");
                if (readProduct != null)
                {
                    readProduct.Price = 1299;
                    readProduct.StockQuantity = 15;
                    productLogic.UpdateProduct(readProduct);
                    Console.WriteLine($"Товар обновлён: новая цена {readProduct.Price}, количество {readProduct.StockQuantity}");
                }
                
                // DELETE - Удаление
                Console.WriteLine("\n4. DELETE - Удаление товара");
                if (productLogic.DeleteProduct(product.Id))
                {
                    Console.WriteLine("Товар успешно удалён");
                }
                
                Console.WriteLine("\nCRUD операции выполнены успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении CRUD операций: {ex.Message}");
            }
        }
    }
}
